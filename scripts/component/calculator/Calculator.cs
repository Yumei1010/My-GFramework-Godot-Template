using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace TimeToTwentyfour.scripts.component.calculator;

[Log]
[ContextAware]
public partial class Calculator : Node, ICalculator
{
    public override void _Ready()
    {
        RegisterEvent();
    }
}