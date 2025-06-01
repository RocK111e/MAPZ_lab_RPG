using MAPZ_lab_RPG.Entities;
using Godot;
using System;
namespace Scenes.Managers
{
    public class AttackCommand: ICommand
    {
        private ICreature _target;
        private Control _targetVisual;

        public Action<ICreature, Control> OnAttackExecuted;

        public AttackCommand(ICreature target, Control targetVisual)
        {
            _target = target;
            _targetVisual = targetVisual;
            GD.Print("AttackCommand initialized.");
        }
        public void Execute()
        {
            GD.Print($"Executing attack on target");
            OnAttackExecuted?.Invoke(_target, _targetVisual);
        }
    }
    
}