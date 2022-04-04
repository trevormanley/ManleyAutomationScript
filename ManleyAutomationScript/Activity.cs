/*
Copyright 2022 Trevor Manley

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Text.RegularExpressions;

namespace ManleyAutomationScript {
    public class Activity {
        public Module Parent {get; internal set;}
        public string Name {get; internal set;}
        public List<Step> Steps {get; internal set;} = new();
        public List<Regex> Expressions { get; internal set; } = new ();
        public string QualifiedName { 
            get {
                return $"{Parent.Name}::{Name}";
            }
        }

        public Activity(Module parent, string name){
            Name = name;
            Parent = parent;
        }

        private Dictionary<Regex,Activity> UsableExpressions = new();
        private Regex RunCSharpExpression = new Regex(@"Run:\n\s*```csharp\n(?<code>.*)```",RegexOptions.Singleline);
        public void Execute(ActivityState state){
            OrganizeUsableExpressions(Parent);
            foreach(var module in state.Modules){
                OrganizeUsableExpressions(module);
            }
            foreach(var step in Steps){
                bool foundAction = false;
                var runMatch = RunCSharpExpression.Match(step.Text);
                if(runMatch.Success){
                    foundAction = true;
                    state.GetExecutedSteps().Add(step.Text);
                    var action = CSharpScriptRunner.CreateFromText(runMatch.Groups["code"].Value);
                    state.LastStep = step;
                    action(state);
                    continue;
                }
                foreach(var (expression,activity) in UsableExpressions){
                    var match = expression.Match(step.Text);
                    if(match.Success){
                        state.GetExecutedSteps().Add(step.Text);
                        state.LastExpression = match;
                        state.LastStep = step;
                        activity.Execute(state);
                        foundAction = true;
                        break;
                    }
                }
                if(!foundAction){
                    // Didn't find anything for it
                    // [#]
                    state.GetExecutedSteps().Add(step.Text);
                }
            }
        }
        private void OrganizeUsableExpressions(Module module){
            foreach(var activity in module.Activities){
                foreach(var expression in activity.Expressions){
                    if(!UsableExpressions.ContainsKey(expression)){
                        UsableExpressions.Add(expression,activity);
                    }
                }
            }
        }
        private bool IsDoingExpressions = false;
        public Regex StepStartingExpression = new Regex(@"^(?:(?:\d+\.)|(?:\*)) (?<text>.*)");
        public void Parse(string line)
        {
            if(line.StartsWith("---")){
                if(IsDoingExpressions){
                    IsDoingExpressions = false;
                    return;
                }
                IsDoingExpressions = true;
                return;
            }
            if(IsDoingExpressions){
                Expressions.Add(new Regex(line,RegexOptions.Singleline));
                return;
            }
            var topLevelStep = StepStartingExpression.Match(line);
            if(topLevelStep.Success){
                Steps.Add(new Step(this){Text = topLevelStep.Groups["text"].Value});
                return;
            }
            if(!Steps.Any()){
                return;
            }
            var lastStep = Steps.Last();
            var spaceCount = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            var depth = spaceCount / 4;
            var trimAmount = 0;
            for(var walk = 0; walk < depth; walk++){
                if(lastStep.Children.Any()){
                    lastStep = lastStep.Children.Last();
                    trimAmount += 4;
                    continue;
                }
                var maybeNewStep = StepStartingExpression.Match(line.Substring(trimAmount + 4));
                if(maybeNewStep.Success){
                    lastStep.Children.Add(new Step(this){Text = maybeNewStep.Groups["text"].Value});
                    return;
                }
            }
            lastStep.Text += "\n" + line.Substring(trimAmount);
        }
    }
}