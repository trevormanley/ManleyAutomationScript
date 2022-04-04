# Manley Automation Script

A Markdown like scripting tool to assist in automation.

## Work Less. Do MAS. Live More.

MAS allows for creation of simple and reusable components that 
can be read in a Markdown preview. 

MAS allows for these components to be combined by non-coders
to create automation.

## Sample Script

```text
---
A copyright statement
---
# Sample

## Activity That Will Execute

1. Add 1 to 5 and store as `total`
2. Add 10 to `total`
3. Print `total`

## Print

---
Print `(?<var>)`
---

1. Run:
    ```csharp
    using System;
    return (ActivityState state) => {
        var groups = state.GetLastExpression().Groups;
        var text = state.Get<object>(groups["var"].Value).ToString();
        Console.WriteLine(text);
    }

## Add

---
Add (?<first>\d*?) to (?<second>\d*?) and store as `(?<result>.*?)`
Add `(?<var1>.*?)` to `(?<var2>.*?)` and store as `(?<result>.*?)`
Add (?<first>\d*?) to `(?<var>.*?)`
---

1. Run:
    ```csharp
    using System.Text.RegularExpressions;
    using System;
    return (ActivityState state) => {
        var groups = state.GetLastExpression().Groups;
        if(groups.ContainsKey("second")){
            var first = Int64.Parse(groups["first"].Value);
            var second = Int64.Parse(groups["second"].Value);
            state.Set<Int64>(groups["result"].Value, first + second);
            return;
        }
        if(groups.ContainsKey("var2")){
            var first = state.Get<Int64>(groups["var1"].Value);
            var second = state.Get<Int64>(groups["var2"].Value);
            state.Set<Int64>(groups["result"].Value, first + second);
            return;
        }
        if(groups.ContainsKey("var")){
            var first = Int64.Parse(groups["first"].Value);
            var second = state.Get<Int64>(groups["var"].Value);
            state.Set<Int64>(groups["var"].Value, first + second);
            return;
        }
    };
    ```
```