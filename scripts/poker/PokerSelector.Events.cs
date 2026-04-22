using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFrameworkGodotTemplate.scripts.enums.calculate;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

public partial class PokerSelector
{
    private void RegisterEvent()
    {
        // 注册对可用性变更事件的监听
        this.RegisterEvent<PokerSelectorEnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择变更事件的监听
        this.RegisterEvent<PokerSelectorSelectChangedEvent>(e =>
        {
            OnSelectChangedEvent(e.Poker,e.IsSelected);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对预览变更事件的监听
        this.RegisterEvent<PokerSelectorReservesChangedEvent>(e =>
        {
            OnReservesChangedEvent(e.Poker,e.IsSelected);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对模式变更事件的监听
        this.RegisterEvent<PokerSelectorModeChangedEvent>(e =>
        {
            OnModeChangedEvent(e.Mode);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        // 更新可用性
        Enable = enable;
        
        // 清除选择对象和预览对象
        Clear();
    }
    
    private void OnSelectChangedEvent(IPoker poker,bool isSelected)
    {
        // 如果选择器未启用，返回
        if (!Enable) return;
        
        // 如果被选中，添加进选择对象，否则，移除
        if (isSelected)
        {
            // 如果选择对象中已经包含，返回
            if (Selects.Contains(poker)) return;
            
            // 如果预览对象已经包含，从预览对象中移除
            if (Reserves.Contains(poker))
            {
                Reserves.Remove(poker);
                
                this.SendEvent(new PokerReserveResultChangedEvent
                {
                    NumValue = "666",
                    IsHidden = false,
                    Poker = poker
                });
            }
            
            // 如果选择对象已满，释放队首
            if (Selects.Count == CurrentMode.GetCapacity())
            {
                this.SendEvent(new PokerStateChangedEvent
                {
                    State = StateType.UnSelect,
                    Poker = Selects[0]
                });
                
                Selects.RemoveAt(0);
            }
                
            Selects.Add(poker);

            // 如果选择对象已满足模式要求，计算
            if (Selects.Count == CurrentMode.GetCapacity())
            {
                GD.Print("本次计算");
                for (int i = 0; i < Selects.Count; i++)
                {
                    GD.Print("第",i+1,"张牌数值为：",Selects[i].GetNumValue());
                }
            
                Calculate();
            }
        }
        else
        {
            // 如果选择对象中未包含，返回
            if (!Selects.Contains(poker)) return;
            
            Selects.Remove(poker);
        }
    }

    private void OnReservesChangedEvent(IPoker poker, bool isSelected)
    {
        // 如果选择器未启用，返回
        if (!Enable) return;
        
        // 如果被选中，添加进预览对象，否则，移除
        if (isSelected)
        {
            // 如果选择对象已经包含，返回
            if (Selects.Contains(poker)) return;
            
            // 如果预览对象已经包含。返回
            if (Reserves.Contains(poker)) return;
            
            Reserves.Add(poker);

            // 如果选择对象与预览对象的数量之和满足模式需求
            if (Selects.Count + Reserves.Count == CurrentMode.GetCapacity())
            {
                IList<IPoker> pokers = new List<IPoker>();
                
                // 如果选择对象的数量不为零，添加进运算负载中
                if (Selects.Count != 0) pokers.Add(Selects[0]);
                
                pokers.Add(Reserves[0]);
                
                CurrentMode.SetPokers(pokers);
                
                this.SendEvent(new PokerReserveResultChangedEvent
                {
                    NumValue = CurrentMode.GetReserveResult(),
                    IsHidden = true,
                    Poker = Reserves[^1]
                });
            }
        }
        else
        {
            // 如果预览对象中未包含，返回
            if (!Reserves.Contains(poker)) return;
            
            this.SendEvent(new PokerReserveResultChangedEvent
            {
                NumValue = "666",
                IsHidden = false,
                Poker = Reserves[^1]
            });
            
            Reserves.Remove(Reserves[^1]);
        }
    }
    
    private void OnModeChangedEvent(ModeType mode)
    {
        ChangeTo(mode);
    }
}