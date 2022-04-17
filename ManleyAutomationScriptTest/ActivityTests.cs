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
using Xunit;
using ManleyAutomationScript;
using System.Linq;
namespace ManleyAutomationScriptTests {
    public class ActivityTests {
        [Fact]
        public void ShouldFork(){
            var module = new Module("Test Module","");
            var activity = new Activity(module, "This is a test");
            var step = new Step(activity);
            var fork = activity.Fork(new System.Collections.Generic.List<Step>(){step});
            Assert.Equal("Test Module::This is a test::Fork", fork.QualifiedName);
        }
        [Fact]
        public void ShouldNotOwnForkedSteps(){
            var module = new Module("Test Module","");
            var activity = new Activity(module, "This is a test");
            var step = new Step(activity);
            var fork = activity.Fork(new System.Collections.Generic.List<Step>(){step});
            Assert.Equal(activity,fork.Steps.Single().OwningActivity);
        }
        [Fact]
        public void ShouldFindActivityFromExpression(){
            var module = new Module("Test Module","");
            var otherActivity = new Activity(module, "Other thing");
            otherActivity.Expressions.Add(new System.Text.RegularExpressions.Regex("Do really cool thing"));
            otherActivity.Steps.Add(new Step(otherActivity){Text="Really Cool Thing"});
            var activity = new Activity(module, "Test");
            activity.Steps.Add(new Step(activity) {Text="Do really cool thing"});
            module.Activities.Add(otherActivity);
            module.Activities.Add(activity);
            var state = new ActivityState();
            state.AddModule(module);
            activity.Execute(state);
            var steps = state.GetExecutedSteps();
            Assert.Equal(2, steps.Count);
            Assert.Equal("Do really cool thing", steps.First());
            Assert.Equal("Really Cool Thing", steps.Last());
        }

        [Fact]
        public void ShouldHaveProperQualifiedName(){
            var module = new Module("Test Module", "Something");
            var activity = new Activity(module, "This is a test");
            Assert.Equal("Test Module::This is a test", activity.QualifiedName);
        }
        [Fact]
        public void ShouldParseRegexSection(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("---");
            activity.Parse("(?<email>.*?@.*?)");
            activity.Parse("---");
            Assert.Equal("(?<email>.*?@.*?)", activity.Expressions.First().ToString());
        }
        [Fact]
        public void ShouldParsesStepAfterBlanks(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("");
            activity.Parse("1. Create a step");
            Assert.Equal("Create a step", activity.Steps.First().Text);
        }
        [Fact]
        public void ShouldParsesStepWithStartingNumber(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            Assert.Equal("Create a step", activity.Steps.First().Text);
        }
        [Fact]
        public void ShouldParsesStepWithStar(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("* Create a step");
            Assert.Equal("Create a step", activity.Steps.First().Text);
        }
        [Fact]
        public void ShouldParseMultiline(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    Here is some additional text");
            Assert.Equal("Create a step\n    Here is some additional text", activity.Steps.First().Text);
        }
        [Fact]
        public void ShouldParseChildSteps(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    * Make a sandwitch");
            Assert.Equal("Make a sandwitch", activity.Steps.First().Children.First().Text);
        }
        [Fact]
        public void ShouldParseChildStepsMultiple(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    * Make a sandwitch");
            activity.Parse("    * Eat sandwitch");
            Assert.Equal("Eat sandwitch", activity.Steps.First().Children.Last().Text);
        }
        [Fact]
        public void ShouldParseChildStepsContinued(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    * Make a sandwitch");
            activity.Parse("        * Eat sandwitch");
            Assert.Equal("Eat sandwitch", activity.Steps.First().Children.First().Children.First().Text);
        }
        [Fact]
        public void ShouldParseChildStepsContinuedMultiline(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    * Make a sandwitch");
            activity.Parse("        There is some other stuff here");
            activity.Parse("            AndSomeHere");
            activity.Parse("        1. Eat sandwitch");
            Assert.Equal("Eat sandwitch", activity.Steps.First().Children.First().Children.First().Text);
        }
        [Fact]
        public void ShouldParseChildStepsMultiline(){
            var module = new Module("Test Module", "");
            var activity = new Activity(module, "This is a test");
            activity.Parse("1. Create a step");
            activity.Parse("    * Line 1");
            activity.Parse("        Line 2");
            activity.Parse("            Line 3");
            activity.Parse("    Line 4");
            Assert.Equal("Line 1\n    Line 2\n        Line 3\nLine 4", activity.Steps.First().Children.First().Text);
        }
    }
}