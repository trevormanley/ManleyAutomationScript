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
using System;
using System.Linq;
public class SampleTests {
    private string GetSampleText(){
        return 
@"
# Common

## Test Adding

1. Add 7 to 70 and store as `result`
2. Add 623 to `result` and store as `result`

## Add

---
Add (?<first>\d*?) to (?<second>\d*?) and store as `(?<result>.*?)`
Add `(?<var1>.*?)` to `(?<var2>.*?)` and store as `(?<result>.*?)`
Add (?<first>\d*?) to `(?<var>.*?)` and store as `(?<result>.*?)`
---

1. Run:
    ```csharp
    using System.Text.RegularExpressions;
    using System;
    return (ActivityState state) => {
        var groups = state.GetLastExpression().Groups;
        if(groups.ContainsKey(""second"")){
            var first = Int64.Parse(groups[""first""].Value);
            var second = Int64.Parse(groups[""second""].Value);
            state.Set<Int64>(groups[""result""].Value, first + second);
            return;
        }
        if(groups.ContainsKey(""var2"")){
            var first = state.Get<Int64>(groups[""var1""].Value);
            var second = state.Get<Int64>(groups[""var2""].Value);
            state.Set<Int64>(groups[""result""].Value, first + second);
            return;
        }
        if(groups.ContainsKey(""var"")){
            var first = Int64.Parse(groups[""first""].Value);
            var second = state.Get<Int64>(groups[""var""].Value);
            state.Set<Int64>(groups[""result""].Value, first + second);
            return;
        }
    };
    ```
";
    }
    private Parser ParseSample()
    {
        var sampleMas = GetSampleText();
        var parser = new Parser();
        parser.Parse(sampleMas);
        return parser;
    }

    [Fact]
    public void ShouldAddNumbers(){
        var parser = ParseSample();
        var state = new ActivityState();
        var act = parser.Modules.Select(x => x.Activities.First(y => y.QualifiedName == "Common::Test Adding")).Single();
        act.Execute(state);
        var result = state.Get<Int64>("result");
        Assert.Equal(700, result);
    }
    [Fact]
    public void ShouldAddModule()
    {
        Parser parser = ParseSample();
        Assert.Equal(1, parser.Modules.Count);
        Assert.Equal("Common", parser.Modules.First().Name);
    }


    [Fact]
    public void ShouldAddActivities(){
        var parser = ParseSample();
        Assert.Equal(2, parser.Modules.First().Activities.Count);
        Assert.Equal("Common::Test Adding", parser.Modules.First().Activities.First().QualifiedName);
        Assert.Equal("Common::Add", parser.Modules.First().Activities.Last().QualifiedName);
    }
}