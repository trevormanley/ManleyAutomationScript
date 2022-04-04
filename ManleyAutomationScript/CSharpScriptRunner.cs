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
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ManleyAutomationScript{
    public static class CSharpScriptRunner {
        public static Action<ActivityState> CreateFromText(string text){
            ScriptOptions options = ScriptOptions.Default
                .WithReferences(typeof(CSharpScriptRunner).Assembly)
                .WithImports(typeof(CSharpScriptRunner).Namespace);
            var scriptProcessor = CSharpScript.Create<Action<ActivityState>>(text,options);
            return scriptProcessor.RunAsync().Result.ReturnValue;
        }
    }
}