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

namespace ManleyAutomationScript{
    public class ActivityState
    {
        private List<string> ExecutedSteps = new List<string>();
        public List<Module> Modules {get;set;} = new List<Module>();
        public Match? LastExpression {get;set;} = null;

        public Step? LastStep {get;set;} = null;
        private Dictionary<string, object?> _internalState = new();
        public void Set<T>(string key, T value){
            _internalState[key] = value;
        }
        public T? Get<T>(string key){
            return (T?) _internalState[key];
        }
        public void AddModule(Module module){
            Modules.Add(module);
        }
        public List<string> GetExecutedSteps(){
            return ExecutedSteps;
        }

        public Match? GetLastExpression(){
            return LastExpression;
        }
    }
}