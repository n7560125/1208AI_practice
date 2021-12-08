using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public enum eBTNodeState
    {
        SUCCEED = 0,
        FAILED,
        RUNNING
    }

    public abstract class cBTNode
    {
        protected eBTNodeState m_NodeState;

        public cBTNode()
        {

        }

        public eBTNodeState NodeState() { return m_NodeState; }
        public abstract void Init();
        public abstract eBTNodeState Process(AIData data);
    }

 

    public class cCompositeNode : cBTNode
    {
        protected List<cBTNode> m_NodeList;
        public cCompositeNode()
        {

        }
        public override void Init()
        {
            m_NodeList = new List<cBTNode>();
            m_NodeState = eBTNodeState.SUCCEED;
        }
        public override eBTNodeState Process(AIData data)
        {
            return m_NodeState;
        }

        public void AddChild(cBTNode node)
        {
            if (m_NodeList.Contains(node))
            {
                return;
            }
            m_NodeList.Add(node);
        }

        public void RemoveChild(cBTNode node)
        {
            if (m_NodeList.Contains(node))
            {
                m_NodeList.Remove(node);
            }
        }
    }

    public class cBTSystem
    {
        private cBTNode m_CurrentNode;
        private cBTNode m_Root;

        public cBTSystem()
        {
            m_CurrentNode = null;
            m_Root = null;
        }

        public void SetRootNode(cBTNode node)
        {
            m_Root = node;
        }

        public void Process(AIData data)
        {
            if(m_CurrentNode == null)
            {
                m_CurrentNode = m_Root;
            }
            if(m_CurrentNode.Process(data) != eBTNodeState.RUNNING)
            {
                m_CurrentNode = m_Root;
            }
        }
    }

    public class cBTLoopNode : cBTNode
    {
        protected List<cBTNode> m_NodeList;
        protected int m_iLoopCount;
        protected int m_iLoop;
        public cBTLoopNode()
        {
            m_iLoopCount = -1;
            m_iLoop = 0;

        }
        public void SetLoopCount(int iLoopCount)
        {
            m_iLoopCount = iLoopCount;
        }
        public override void Init()
        {
            m_NodeList = new List<cBTNode>();
            m_NodeState = eBTNodeState.RUNNING;
        }
        public override eBTNodeState Process(AIData data)
        {

            int i = 0;
            int iCount = m_NodeList.Count;
            eBTNodeState state;
       
            for (i = 0; i < iCount; i++)
            {
                state = m_NodeList[i].Process(data);
                if (state == eBTNodeState.SUCCEED)
                {
                    continue;
                }
                else if (state == eBTNodeState.FAILED)
                {
                    continue;
                }
                else
                {
                    return eBTNodeState.RUNNING;
                }
            }
            data.m_BTSystem.SetRootNode(this);
            if (m_iLoopCount < 0)
            {
                return eBTNodeState.RUNNING;
            }
            m_iLoop++;
            if(m_iLoop < m_iLoopCount)
            {
                return eBTNodeState.RUNNING;
            } else
            {
                return eBTNodeState.SUCCEED;
            }

        }

        public void AddChild(cBTNode node)
        {
            if (m_NodeList.Contains(node))
            {
                return;
            }
            m_NodeList.Add(node);
        }

        public void RemoveChild(cBTNode node)
        {
            if (m_NodeList.Contains(node))
            {
                m_NodeList.Remove(node);
            }
        }
    }

    public class cBTActionNode : cBTNode
    {
        public delegate eBTNodeState ActionNodeDelegate(AIData data);
        private ActionNodeDelegate m_Action;

        public void SetAction(ActionNodeDelegate action)
        {
            m_Action = action;
        }

        public override void Init()
        {
            m_NodeState = eBTNodeState.SUCCEED;
        }

        public override eBTNodeState Process(AIData data)
        {
            return m_Action(data);
        }
    }

    public class cBTConditionNode : cBTNode
    {
        public delegate eBTNodeState ConditionNodeDelegate(AIData data);
        private ConditionNodeDelegate m_Condition;

        public void SetConditionCheck(ConditionNodeDelegate action)
        {
            m_Condition = action;
        }

        public override void Init()
        {
            m_NodeState = eBTNodeState.SUCCEED;
        }

        public override eBTNodeState Process(AIData data)
        {
            return m_Condition(data);
        }
    }

    public class cBTSelector : cCompositeNode
    {
        public override void Init()
        {
            base.Init();
        }

        public override eBTNodeState Process(AIData data)
        {
            int i = 0;
            int iCount = m_NodeList.Count;
            eBTNodeState state;
            eBTNodeState rstate = eBTNodeState.FAILED;
            for (i = 0; i < iCount; i++)
            {
                state = m_NodeList[i].Process(data);
                if (state == eBTNodeState.SUCCEED)
                {
                    return eBTNodeState.SUCCEED;
                } else if (state == eBTNodeState.FAILED)
                {
                    continue;
                } else
                {
                    return eBTNodeState.RUNNING;
                }
            }
            return rstate;
        }
    }

    public class cBTSequence : cCompositeNode
    {
       
        public override void Init()
        {
            base.Init();
        }

        public override eBTNodeState Process(AIData data)
        {
            int i = 0;
            int iCount = m_NodeList.Count;
            eBTNodeState state;
            eBTNodeState rstate = eBTNodeState.FAILED;
            for (i = 0; i < iCount; i++)
            {
                state = m_NodeList[i].Process(data);
                if (state == eBTNodeState.SUCCEED)
                {
                    continue;
                }
                else if (state == eBTNodeState.FAILED)
                {
                    return eBTNodeState.FAILED;
                }
                else
                {
                    return eBTNodeState.RUNNING;
                }
            }
            return rstate;
        }

        
    }
}
