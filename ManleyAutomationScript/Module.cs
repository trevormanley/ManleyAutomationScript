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
namespace ManleyAutomationScript
{
    public class Module {
        public string Name {get; internal set;}
        public string CopyrightStatement {get; internal set;}
        public List<Activity> Activities {get; internal set;} = new();

        public Module(string name, string copyrightStatement){
            Name = name;
            CopyrightStatement = copyrightStatement;
        }

        public void Parse(string line)
        {
            if(line.StartsWith("## ")){
                var activityName = line.Replace("## ","").Trim();
                Activities.Add(new Activity(this, activityName));
                return; 
            }
            if(!Activities.Any()){
                return;
            }
            Activities.Last().Parse(line);
        }
    }
}