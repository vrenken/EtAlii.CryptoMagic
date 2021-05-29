// Remark: this file was auto-generated based on 'CircularSequence.puml'.
// Any changes will be overwritten the next time the file is generated.

namespace EtAlii.BinanceMagic.Service
{
    using System;
    using System.Threading.Tasks;
    using Stateless;
    
    /// <summary>
    /// This is the base class for the state machine as defined in 'CircularSequence.puml'.
    /// Inherit the class and override the transition methods to define the necessary business behavior.
    /// The transitions can then be triggered by calling the corresponding trigger methods.
    /// </summary>
    public abstract class CircularSequenceBase
    {
        protected global::Stateless.StateMachine<State, Trigger> StateMachine => _stateMachine;
        private readonly global::Stateless.StateMachine<State, Trigger> _stateMachine;
        
        
        protected CircularSequenceBase()
        {
            // Time to create a new state machine instance.
            _stateMachine = new global::Stateless.StateMachine<State, Trigger>(State._Begin);
            
            // Then we need to configure the state machine.
            _stateMachine.Configure(State._Begin)
            	.OnEntry(On_BeginEntered)
            	.OnExit(On_BeginExited)
            	.Permit(Trigger.Start, State.LoadPreviousCycleFromDatabase);
            
            _stateMachine.Configure(State._End)
            	.OnEntry(On_EndEntered)
            	.OnEntryFrom(Trigger.Continue, On_EndEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, On_EndEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, On_EndEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, On_EndEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnEntryFrom(Trigger.No, On_EndEnteredFromNoTrigger)
            	.OnExit(On_EndExited);
            
            _stateMachine.Configure(State.BuyAInInitialCycle)
            	.OnEntry(OnBuyAInInitialCycleEntered)
            	.OnEntryFrom(Trigger.Yes, OnBuyAInInitialCycleEnteredFromYesTrigger)
            	.OnExit(OnBuyAInInitialCycleExited)
            	.Permit(Trigger.Continue, State._End)
            	.SubstateOf(State.InitialPurchaseOfA);
            
            _stateMachine.Configure(State.SellABuyBInInitialCycle)
            	.OnEntry(OnSellABuyBInInitialCycleEntered)
            	.OnEntryFrom(Trigger.Yes, OnSellABuyBInInitialCycleEnteredFromYesTrigger)
            	.OnExit(OnSellABuyBInInitialCycleExited)
            	.Permit(Trigger.Continue, State._End)
            	.SubstateOf(State.InitialPurchaseOfB);
            
            _stateMachine.Configure(State.CheckIfSufficientA)
            	.OnEntry(OnCheckIfSufficientAEntered)
            	.OnEntryFrom(Trigger.Yes, OnCheckIfSufficientAEnteredFromYesTrigger)
            	.OnExit(OnCheckIfSufficientAExited)
            	.Permit(Trigger.Continue, State.HasSufficientA)
            	.SubstateOf(State.TransferFromAToB);
            
            _stateMachine.Configure(State.CheckIfSufficientB)
            	.OnEntry(OnCheckIfSufficientBEntered)
            	.OnEntryFrom(Trigger.Yes, OnCheckIfSufficientBEnteredFromYesTrigger)
            	.OnExit(OnCheckIfSufficientBExited)
            	.Permit(Trigger.Continue, State.HasSufficientB)
            	.SubstateOf(State.TransferFromBToA);
            
            _stateMachine.Configure(State.CheckIfSufficientReferenceInInitialPurchaseOfA)
            	.OnEntry(OnCheckIfSufficientReferenceInInitialPurchaseOfAEntered)
            	.OnEntryFrom(Trigger._BeginToCheckIfSufficientReferenceInInitialPurchaseOfA, OnCheckIfSufficientReferenceInInitialPurchaseOfAEnteredFrom_BeginToCheckIfSufficientReferenceInInitialPurchaseOfATrigger)
            	.OnExit(OnCheckIfSufficientReferenceInInitialPurchaseOfAExited)
            	.Permit(Trigger.Continue, State.HasSufficientReferenceInInitialPurchaseOfA)
            	.SubstateOf(State.InitialPurchaseOfA);
            
            _stateMachine.Configure(State.CheckIfSufficientReferenceInInitialPurchaseOfB)
            	.OnEntry(OnCheckIfSufficientReferenceInInitialPurchaseOfBEntered)
            	.OnEntryFrom(Trigger._BeginToCheckIfSufficientReferenceInInitialPurchaseOfB, OnCheckIfSufficientReferenceInInitialPurchaseOfBEnteredFrom_BeginToCheckIfSufficientReferenceInInitialPurchaseOfBTrigger)
            	.OnExit(OnCheckIfSufficientReferenceInInitialPurchaseOfBExited)
            	.Permit(Trigger.Continue, State.HasSufficientReferenceInInitialPurchaseOfB)
            	.SubstateOf(State.InitialPurchaseOfB);
            
            _stateMachine.Configure(State.CheckWhatCycle)
            	.OnEntry(() => OnCheckWhatCycleEntered(new CheckWhatCycleEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnCheckWhatCycleEnteredFromContinueTrigger(new CheckWhatCycleEventArgs(this)))
            	.OnExit(OnCheckWhatCycleExited)
            	.Permit(Trigger.IsInitialCycleToA, State.InitialPurchaseOfA)
            	.Permit(Trigger.IsInitialCycleToB, State.InitialPurchaseOfB)
            	.Permit(Trigger.IsNormalCycleFromAToB, State.TransferFromAToB)
            	.Permit(Trigger.IsNormalCycleFromBToA, State.TransferFromBToA);
            
            _stateMachine.Configure(State.GetSituationInTransferFromAToB)
            	.OnEntry(OnGetSituationInTransferFromAToBEntered)
            	.OnEntryFrom(Trigger._BeginToGetSituationInTransferFromAToB, OnGetSituationInTransferFromAToBEnteredFrom_BeginToGetSituationInTransferFromAToBTrigger)
            	.OnExit(OnGetSituationInTransferFromAToBExited)
            	.Permit(Trigger.Continue, State.TransferFromAToBIsWorthIt)
            	.SubstateOf(State.TransferFromAToB);
            
            _stateMachine.Configure(State.GetSituationInTransferFromBToA)
            	.OnEntry(OnGetSituationInTransferFromBToAEntered)
            	.OnEntryFrom(Trigger._BeginToGetSituationInTransferFromBToA, OnGetSituationInTransferFromBToAEnteredFrom_BeginToGetSituationInTransferFromBToATrigger)
            	.OnExit(OnGetSituationInTransferFromBToAExited)
            	.Permit(Trigger.Continue, State.TransferFromBToAIsWorthIt)
            	.SubstateOf(State.TransferFromBToA);
            
            _stateMachine.Configure(State.HasSufficientA)
            	.OnEntry(() => OnHasSufficientAEntered(new HasSufficientAEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnHasSufficientAEnteredFromContinueTrigger(new HasSufficientAEventArgs(this)))
            	.OnExit(OnHasSufficientAExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.SellABuyB)
            	.SubstateOf(State.TransferFromAToB);
            
            _stateMachine.Configure(State.HasSufficientB)
            	.OnEntry(() => OnHasSufficientBEntered(new HasSufficientBEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnHasSufficientBEnteredFromContinueTrigger(new HasSufficientBEventArgs(this)))
            	.OnExit(OnHasSufficientBExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.SellBBuyA)
            	.SubstateOf(State.TransferFromBToA);
            
            _stateMachine.Configure(State.HasSufficientReferenceInInitialPurchaseOfA)
            	.OnEntry(() => OnHasSufficientReferenceInInitialPurchaseOfAEntered(new HasSufficientReferenceInInitialPurchaseOfAEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnHasSufficientReferenceInInitialPurchaseOfAEnteredFromContinueTrigger(new HasSufficientReferenceInInitialPurchaseOfAEventArgs(this)))
            	.OnExit(OnHasSufficientReferenceInInitialPurchaseOfAExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.BuyAInInitialCycle)
            	.SubstateOf(State.InitialPurchaseOfA);
            
            _stateMachine.Configure(State.HasSufficientReferenceInInitialPurchaseOfB)
            	.OnEntry(() => OnHasSufficientReferenceInInitialPurchaseOfBEntered(new HasSufficientReferenceInInitialPurchaseOfBEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnHasSufficientReferenceInInitialPurchaseOfBEnteredFromContinueTrigger(new HasSufficientReferenceInInitialPurchaseOfBEventArgs(this)))
            	.OnExit(OnHasSufficientReferenceInInitialPurchaseOfBExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.SellABuyBInInitialCycle)
            	.SubstateOf(State.InitialPurchaseOfB);
            
            _stateMachine.Configure(State.InitialPurchaseOfA)
            	.InitialTransition(State.CheckIfSufficientReferenceInInitialPurchaseOfA)
            	.OnEntry(OnInitialPurchaseOfAEntered)
            	.OnEntryFrom(Trigger.IsInitialCycleToA, OnInitialPurchaseOfAEnteredFromIsInitialCycleToATrigger)
            	.OnExit(OnInitialPurchaseOfAExited)
            	.Permit(Trigger.Continue, State.Wait);
            
            _stateMachine.Configure(State.InitialPurchaseOfB)
            	.InitialTransition(State.CheckIfSufficientReferenceInInitialPurchaseOfB)
            	.OnEntry(OnInitialPurchaseOfBEntered)
            	.OnEntryFrom(Trigger.IsInitialCycleToB, OnInitialPurchaseOfBEnteredFromIsInitialCycleToBTrigger)
            	.OnExit(OnInitialPurchaseOfBExited)
            	.Permit(Trigger.Continue, State.Wait);
            
            _stateMachine.Configure(State.LoadPreviousCycleFromDatabase)
            	.OnEntry(OnLoadPreviousCycleFromDatabaseEntered)
            	.OnEntryFrom(Trigger.Start, OnLoadPreviousCycleFromDatabaseEnteredFromStartTrigger)
            	.OnExit(OnLoadPreviousCycleFromDatabaseExited)
            	.Permit(Trigger.Continue, State.CheckWhatCycle);
            
            _stateMachine.Configure(State.SellABuyB)
            	.OnEntry(OnSellABuyBEntered)
            	.OnEntryFrom(Trigger.Yes, OnSellABuyBEnteredFromYesTrigger)
            	.OnExit(OnSellABuyBExited)
            	.Permit(Trigger.Continue, State._End)
            	.SubstateOf(State.TransferFromAToB);
            
            _stateMachine.Configure(State.SellBBuyA)
            	.OnEntry(OnSellBBuyAEntered)
            	.OnEntryFrom(Trigger.Yes, OnSellBBuyAEnteredFromYesTrigger)
            	.OnExit(OnSellBBuyAExited)
            	.Permit(Trigger.Continue, State._End)
            	.SubstateOf(State.TransferFromBToA);
            
            _stateMachine.Configure(State.TransferFromAToB)
            	.InitialTransition(State.GetSituationInTransferFromAToB)
            	.OnEntry(OnTransferFromAToBEntered)
            	.OnEntryFrom(Trigger.IsNormalCycleFromAToB, OnTransferFromAToBEnteredFromIsNormalCycleFromAToBTrigger)
            	.OnExit(OnTransferFromAToBExited)
            	.Permit(Trigger.Continue, State.Wait);
            
            _stateMachine.Configure(State.TransferFromAToBIsWorthIt)
            	.OnEntry(() => OnTransferFromAToBIsWorthItEntered(new TransferFromAToBIsWorthItEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnTransferFromAToBIsWorthItEnteredFromContinueTrigger(new TransferFromAToBIsWorthItEventArgs(this)))
            	.OnExit(OnTransferFromAToBIsWorthItExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.CheckIfSufficientA)
            	.SubstateOf(State.TransferFromAToB);
            
            _stateMachine.Configure(State.TransferFromBToA)
            	.InitialTransition(State.GetSituationInTransferFromBToA)
            	.OnEntry(OnTransferFromBToAEntered)
            	.OnEntryFrom(Trigger.IsNormalCycleFromBToA, OnTransferFromBToAEnteredFromIsNormalCycleFromBToATrigger)
            	.OnExit(OnTransferFromBToAExited)
            	.Permit(Trigger.Continue, State.Wait);
            
            _stateMachine.Configure(State.TransferFromBToAIsWorthIt)
            	.OnEntry(() => OnTransferFromBToAIsWorthItEntered(new TransferFromBToAIsWorthItEventArgs(this)))
            	.OnEntryFrom(Trigger.Continue, () => OnTransferFromBToAIsWorthItEnteredFromContinueTrigger(new TransferFromBToAIsWorthItEventArgs(this)))
            	.OnExit(OnTransferFromBToAIsWorthItExited)
            	.Permit(Trigger.No, State._End)
            	.Permit(Trigger.Yes, State.CheckIfSufficientB)
            	.SubstateOf(State.TransferFromBToA);
            
            _stateMachine.Configure(State.Wait)
            	.OnEntry(OnWaitEntered)
            	.OnEntryFrom(Trigger.Continue, OnWaitEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, OnWaitEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, OnWaitEnteredFromContinueTrigger)
            	.OnEntryFrom(Trigger.Continue, OnWaitEnteredFromContinueTrigger)
            	.OnExit(OnWaitExited);
            
        }
        
        // The methods below can be each called to fire a specific trigger
        // and cause the state machine to transition to another state.
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// _Begin --&gt; CheckIfSufficientReferenceInInitialPurchaseOfA : _BeginToCheckIfSufficientReferenceInInitialPurchaseOfA<br/>
        /// </summary>
        public void _BeginToCheckIfSufficientReferenceInInitialPurchaseOfA() => _stateMachine.Fire(Trigger._BeginToCheckIfSufficientReferenceInInitialPurchaseOfA);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// _Begin --&gt; CheckIfSufficientReferenceInInitialPurchaseOfB : _BeginToCheckIfSufficientReferenceInInitialPurchaseOfB<br/>
        /// </summary>
        public void _BeginToCheckIfSufficientReferenceInInitialPurchaseOfB() => _stateMachine.Fire(Trigger._BeginToCheckIfSufficientReferenceInInitialPurchaseOfB);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// _Begin --&gt; GetSituationInTransferFromAToB : _BeginToGetSituationInTransferFromAToB<br/>
        /// </summary>
        public void _BeginToGetSituationInTransferFromAToB() => _stateMachine.Fire(Trigger._BeginToGetSituationInTransferFromAToB);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// _Begin --&gt; GetSituationInTransferFromBToA : _BeginToGetSituationInTransferFromBToA<br/>
        /// </summary>
        public void _BeginToGetSituationInTransferFromBToA() => _stateMachine.Fire(Trigger._BeginToGetSituationInTransferFromBToA);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// LoadPreviousCycleFromDatabase --&gt; CheckWhatCycle : Continue<br/>
        /// InitialPurchaseOfA --&gt; Wait : Continue<br/>
        /// InitialPurchaseOfB --&gt; Wait : Continue<br/>
        /// TransferFromAToB --&gt; Wait : Continue<br/>
        /// TransferFromBToA --&gt; Wait : Continue<br/>
        /// CheckIfSufficientReferenceInInitialPurchaseOfB --&gt; HasSufficientReferenceInInitialPurchaseOfB : Continue<br/>
        /// BuyBInInitialCycle --&gt; _End : Continue<br/>
        /// CheckIfSufficientReferenceInInitialPurchaseOfA --&gt; HasSufficientReferenceInInitialPurchaseOfA : Continue<br/>
        /// BuyAInInitialCycle --&gt; _End : Continue<br/>
        /// GetSituationInTransferFromAToB --&gt; TransferFromAToBIsWorthIt : Continue<br/>
        /// CheckIfSufficientA --&gt; HasSufficientA : Continue<br/>
        /// SellABuyB --&gt; _End : Continue<br/>
        /// GetSituationInTransferFromBToA --&gt; TransferFromBToAIsWorthIt : Continue<br/>
        /// CheckIfSufficientB --&gt; HasSufficientB : Continue<br/>
        /// SellBBuyA --&gt; _End : Continue<br/>
        /// </summary>
        public void Continue() => _stateMachine.Fire(Trigger.Continue);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// CheckWhatCycle --&gt; InitialPurchaseOfA : IsInitialCycleToA<br/>
        /// </summary>
        public void IsInitialCycleToA() => _stateMachine.Fire(Trigger.IsInitialCycleToA);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// CheckWhatCycle --&gt; InitialPurchaseOfB : IsInitialCycleToB<br/>
        /// </summary>
        public void IsInitialCycleToB() => _stateMachine.Fire(Trigger.IsInitialCycleToB);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// CheckWhatCycle --&gt; TransferFromAToB : IsNormalCycleFromAToB<br/>
        /// </summary>
        public void IsNormalCycleFromAToB() => _stateMachine.Fire(Trigger.IsNormalCycleFromAToB);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// CheckWhatCycle --&gt; TransferFromBToA : IsNormalCycleFromBToA<br/>
        /// </summary>
        public void IsNormalCycleFromBToA() => _stateMachine.Fire(Trigger.IsNormalCycleFromBToA);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// HasSufficientReferenceInInitialPurchaseOfB --&gt; _End : No<br/>
        /// HasSufficientReferenceInInitialPurchaseOfA --&gt; _End : No<br/>
        /// TransferFromAToBIsWorthIt --&gt; _End : No<br/>
        /// HasSufficientA --&gt; _End : No<br/>
        /// TransferFromBToAIsWorthIt --&gt; _End : No<br/>
        /// HasSufficientB --&gt; _End : No<br/>
        /// </summary>
        public void No() => _stateMachine.Fire(Trigger.No);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// _Begin --&gt; LoadPreviousCycleFromDatabase : Start<br/>
        /// </summary>
        public void Start() => _stateMachine.Fire(Trigger.Start);
        
        /// <summary>
        /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
        /// HasSufficientReferenceInInitialPurchaseOfB --&gt; BuyBInInitialCycle : Yes<br/>
        /// HasSufficientReferenceInInitialPurchaseOfA --&gt; BuyAInInitialCycle : Yes<br/>
        /// TransferFromAToBIsWorthIt --&gt; CheckIfSufficientA : Yes<br/>
        /// HasSufficientA --&gt; SellABuyB : Yes<br/>
        /// TransferFromBToAIsWorthIt --&gt; CheckIfSufficientB : Yes<br/>
        /// HasSufficientB --&gt; SellBBuyA : Yes<br/>
        /// </summary>
        public void Yes() => _stateMachine.Fire(Trigger.Yes);
        
        
        // The classes below represent the EventArgs as used by some of the methods.
        
        protected class CheckWhatCycleEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public CheckWhatCycleEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// CheckWhatCycle --&gt; InitialPurchaseOfA : IsInitialCycleToA<br/>
            /// </summary>
            public void IsInitialCycleToA() => _stateMachine.IsInitialCycleToA();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// CheckWhatCycle --&gt; InitialPurchaseOfB : IsInitialCycleToB<br/>
            /// </summary>
            public void IsInitialCycleToB() => _stateMachine.IsInitialCycleToB();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// CheckWhatCycle --&gt; TransferFromAToB : IsNormalCycleFromAToB<br/>
            /// </summary>
            public void IsNormalCycleFromAToB() => _stateMachine.IsNormalCycleFromAToB();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// CheckWhatCycle --&gt; TransferFromBToA : IsNormalCycleFromBToA<br/>
            /// </summary>
            public void IsNormalCycleFromBToA() => _stateMachine.IsNormalCycleFromBToA();
            
        }
        
        protected class HasSufficientReferenceInInitialPurchaseOfBEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public HasSufficientReferenceInInitialPurchaseOfBEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientReferenceInInitialPurchaseOfB --&gt; BuyBInInitialCycle : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientReferenceInInitialPurchaseOfB --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
        }
        
        protected class HasSufficientReferenceInInitialPurchaseOfAEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public HasSufficientReferenceInInitialPurchaseOfAEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientReferenceInInitialPurchaseOfA --&gt; BuyAInInitialCycle : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientReferenceInInitialPurchaseOfA --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
        }
        
        protected class TransferFromAToBIsWorthItEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public TransferFromAToBIsWorthItEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// TransferFromAToBIsWorthIt --&gt; CheckIfSufficientA : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// TransferFromAToBIsWorthIt --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
        }
        
        protected class HasSufficientAEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public HasSufficientAEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientA --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientA --&gt; SellABuyB : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
        }
        
        protected class TransferFromBToAIsWorthItEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public TransferFromBToAIsWorthItEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// TransferFromBToAIsWorthIt --&gt; CheckIfSufficientB : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// TransferFromBToAIsWorthIt --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
        }
        
        protected class HasSufficientBEventArgs
        {
            private readonly CircularSequenceBase _stateMachine;
            
            public HasSufficientBEventArgs(CircularSequenceBase stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientB --&gt; _End : No<br/>
            /// </summary>
            public void No() => _stateMachine.No();
            
            /// <summary>
            /// Depending on the current state, call this method to trigger one of the sync transitions below:<br/>
            /// HasSufficientB --&gt; SellBBuyA : Yes<br/>
            /// </summary>
            public void Yes() => _stateMachine.Yes();
            
        }
        
        
        // Of course each state machine needs a set of states.
        protected enum State
        {
            _Begin,
            _End,
            BuyAInInitialCycle,
            SellABuyBInInitialCycle,
            CheckIfSufficientA,
            CheckIfSufficientB,
            CheckIfSufficientReferenceInInitialPurchaseOfA,
            CheckIfSufficientReferenceInInitialPurchaseOfB,
            CheckWhatCycle,
            GetSituationInTransferFromAToB,
            GetSituationInTransferFromBToA,
            HasSufficientA,
            HasSufficientB,
            HasSufficientReferenceInInitialPurchaseOfA,
            HasSufficientReferenceInInitialPurchaseOfB,
            InitialPurchaseOfA,
            InitialPurchaseOfB,
            LoadPreviousCycleFromDatabase,
            SellABuyB,
            SellBBuyA,
            TransferFromAToB,
            TransferFromAToBIsWorthIt,
            TransferFromBToA,
            TransferFromBToAIsWorthIt,
            Wait,
        }
        
        // And all state machine need something that trigger them.
        protected enum Trigger
        {
            _BeginToCheckIfSufficientReferenceInInitialPurchaseOfA,
            _BeginToCheckIfSufficientReferenceInInitialPurchaseOfB,
            _BeginToGetSituationInTransferFromAToB,
            _BeginToGetSituationInTransferFromBToA,
            Continue,
            IsInitialCycleToA,
            IsInitialCycleToB,
            IsNormalCycleFromAToB,
            IsNormalCycleFromBToA,
            No,
            Start,
            Yes,
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the '_Begin' state.
        /// </summary>
        protected virtual void On_BeginEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the '_Begin' state.
        /// </summary>
        protected virtual void On_BeginExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the '_End' state.
        /// </summary>
        protected virtual void On_EndEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the '_End' state.
        /// </summary>
        protected virtual void On_EndExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// HasSufficientReferenceInInitialPurchaseOfB --&gt; _End : No<br/>
        /// </summary>
        protected virtual void On_EndEnteredFromNoTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// BuyBInInitialCycle --&gt; _End : Continue<br/>
        /// </summary>
        protected virtual void On_EndEnteredFromContinueTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'BuyAInInitialCycle' state.
        /// </summary>
        protected virtual void OnBuyAInInitialCycleEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'BuyAInInitialCycle' state.
        /// </summary>
        protected virtual void OnBuyAInInitialCycleExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// HasSufficientReferenceInInitialPurchaseOfA --&gt; BuyAInInitialCycle : Yes<br/>
        /// </summary>
        protected virtual void OnBuyAInInitialCycleEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'BuyBInInitialCycle' state.
        /// </summary>
        protected virtual void OnSellABuyBInInitialCycleEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'BuyBInInitialCycle' state.
        /// </summary>
        protected virtual void OnSellABuyBInInitialCycleExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// HasSufficientReferenceInInitialPurchaseOfB --&gt; BuyBInInitialCycle : Yes<br/>
        /// </summary>
        protected virtual void OnSellABuyBInInitialCycleEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'CheckIfSufficientA' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'CheckIfSufficientA' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// TransferFromAToBIsWorthIt --&gt; CheckIfSufficientA : Yes<br/>
        /// </summary>
        protected virtual void OnCheckIfSufficientAEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'CheckIfSufficientB' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'CheckIfSufficientB' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// TransferFromBToAIsWorthIt --&gt; CheckIfSufficientB : Yes<br/>
        /// </summary>
        protected virtual void OnCheckIfSufficientBEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'CheckIfSufficientReferenceInInitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'CheckIfSufficientReferenceInInitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; CheckIfSufficientReferenceInInitialPurchaseOfA : _BeginToCheckIfSufficientReferenceInInitialPurchaseOfA<br/>
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfAEnteredFrom_BeginToCheckIfSufficientReferenceInInitialPurchaseOfATrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'CheckIfSufficientReferenceInInitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'CheckIfSufficientReferenceInInitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; CheckIfSufficientReferenceInInitialPurchaseOfB : _BeginToCheckIfSufficientReferenceInInitialPurchaseOfB<br/>
        /// </summary>
        protected virtual void OnCheckIfSufficientReferenceInInitialPurchaseOfBEnteredFrom_BeginToCheckIfSufficientReferenceInInitialPurchaseOfBTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'CheckWhatCycle' state.
        /// </summary>
        protected virtual void OnCheckWhatCycleEntered(CheckWhatCycleEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'CheckWhatCycle' state.
        /// </summary>
        protected virtual void OnCheckWhatCycleExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// LoadPreviousCycleFromDatabase --&gt; CheckWhatCycle : Continue<br/>
        /// </summary>
        protected virtual void OnCheckWhatCycleEnteredFromContinueTrigger(CheckWhatCycleEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'GetSituationInTransferFromAToB' state.
        /// </summary>
        protected virtual void OnGetSituationInTransferFromAToBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'GetSituationInTransferFromAToB' state.
        /// </summary>
        protected virtual void OnGetSituationInTransferFromAToBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; GetSituationInTransferFromAToB : _BeginToGetSituationInTransferFromAToB<br/>
        /// </summary>
        protected virtual void OnGetSituationInTransferFromAToBEnteredFrom_BeginToGetSituationInTransferFromAToBTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'GetSituationInTransferFromBToA' state.
        /// </summary>
        protected virtual void OnGetSituationInTransferFromBToAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'GetSituationInTransferFromBToA' state.
        /// </summary>
        protected virtual void OnGetSituationInTransferFromBToAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; GetSituationInTransferFromBToA : _BeginToGetSituationInTransferFromBToA<br/>
        /// </summary>
        protected virtual void OnGetSituationInTransferFromBToAEnteredFrom_BeginToGetSituationInTransferFromBToATrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'HasSufficientA' state.
        /// </summary>
        protected virtual void OnHasSufficientAEntered(HasSufficientAEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'HasSufficientA' state.
        /// </summary>
        protected virtual void OnHasSufficientAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckIfSufficientA --&gt; HasSufficientA : Continue<br/>
        /// </summary>
        protected virtual void OnHasSufficientAEnteredFromContinueTrigger(HasSufficientAEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'HasSufficientB' state.
        /// </summary>
        protected virtual void OnHasSufficientBEntered(HasSufficientBEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'HasSufficientB' state.
        /// </summary>
        protected virtual void OnHasSufficientBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckIfSufficientB --&gt; HasSufficientB : Continue<br/>
        /// </summary>
        protected virtual void OnHasSufficientBEnteredFromContinueTrigger(HasSufficientBEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'HasSufficientReferenceInInitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfAEntered(HasSufficientReferenceInInitialPurchaseOfAEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'HasSufficientReferenceInInitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckIfSufficientReferenceInInitialPurchaseOfA --&gt; HasSufficientReferenceInInitialPurchaseOfA : Continue<br/>
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfAEnteredFromContinueTrigger(HasSufficientReferenceInInitialPurchaseOfAEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'HasSufficientReferenceInInitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfBEntered(HasSufficientReferenceInInitialPurchaseOfBEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'HasSufficientReferenceInInitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckIfSufficientReferenceInInitialPurchaseOfB --&gt; HasSufficientReferenceInInitialPurchaseOfB : Continue<br/>
        /// </summary>
        protected virtual void OnHasSufficientReferenceInInitialPurchaseOfBEnteredFromContinueTrigger(HasSufficientReferenceInInitialPurchaseOfBEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'InitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnInitialPurchaseOfAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'InitialPurchaseOfA' state.
        /// </summary>
        protected virtual void OnInitialPurchaseOfAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckWhatCycle --&gt; InitialPurchaseOfA : IsInitialCycleToA<br/>
        /// </summary>
        protected virtual void OnInitialPurchaseOfAEnteredFromIsInitialCycleToATrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'InitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnInitialPurchaseOfBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'InitialPurchaseOfB' state.
        /// </summary>
        protected virtual void OnInitialPurchaseOfBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckWhatCycle --&gt; InitialPurchaseOfB : IsInitialCycleToB<br/>
        /// </summary>
        protected virtual void OnInitialPurchaseOfBEnteredFromIsInitialCycleToBTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'LoadPreviousCycleFromDatabase' state.
        /// </summary>
        protected virtual void OnLoadPreviousCycleFromDatabaseEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'LoadPreviousCycleFromDatabase' state.
        /// </summary>
        protected virtual void OnLoadPreviousCycleFromDatabaseExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// _Begin --&gt; LoadPreviousCycleFromDatabase : Start<br/>
        /// </summary>
        protected virtual void OnLoadPreviousCycleFromDatabaseEnteredFromStartTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'SellABuyB' state.
        /// </summary>
        protected virtual void OnSellABuyBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellABuyB' state.
        /// </summary>
        protected virtual void OnSellABuyBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// HasSufficientA --&gt; SellABuyB : Yes<br/>
        /// </summary>
        protected virtual void OnSellABuyBEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'SellBBuyA' state.
        /// </summary>
        protected virtual void OnSellBBuyAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'SellBBuyA' state.
        /// </summary>
        protected virtual void OnSellBBuyAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// HasSufficientB --&gt; SellBBuyA : Yes<br/>
        /// </summary>
        protected virtual void OnSellBBuyAEnteredFromYesTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'TransferFromAToB' state.
        /// </summary>
        protected virtual void OnTransferFromAToBEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferFromAToB' state.
        /// </summary>
        protected virtual void OnTransferFromAToBExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckWhatCycle --&gt; TransferFromAToB : IsNormalCycleFromAToB<br/>
        /// </summary>
        protected virtual void OnTransferFromAToBEnteredFromIsNormalCycleFromAToBTrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'TransferFromAToBIsWorthIt' state.
        /// </summary>
        protected virtual void OnTransferFromAToBIsWorthItEntered(TransferFromAToBIsWorthItEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferFromAToBIsWorthIt' state.
        /// </summary>
        protected virtual void OnTransferFromAToBIsWorthItExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// GetSituationInTransferFromAToB --&gt; TransferFromAToBIsWorthIt : Continue<br/>
        /// </summary>
        protected virtual void OnTransferFromAToBIsWorthItEnteredFromContinueTrigger(TransferFromAToBIsWorthItEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'TransferFromBToA' state.
        /// </summary>
        protected virtual void OnTransferFromBToAEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferFromBToA' state.
        /// </summary>
        protected virtual void OnTransferFromBToAExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// CheckWhatCycle --&gt; TransferFromBToA : IsNormalCycleFromBToA<br/>
        /// </summary>
        protected virtual void OnTransferFromBToAEnteredFromIsNormalCycleFromBToATrigger()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'TransferFromBToAIsWorthIt' state.
        /// </summary>
        protected virtual void OnTransferFromBToAIsWorthItEntered(TransferFromBToAIsWorthItEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'TransferFromBToAIsWorthIt' state.
        /// </summary>
        protected virtual void OnTransferFromBToAIsWorthItExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// GetSituationInTransferFromBToA --&gt; TransferFromBToAIsWorthIt : Continue<br/>
        /// </summary>
        protected virtual void OnTransferFromBToAIsWorthItEnteredFromContinueTrigger(TransferFromBToAIsWorthItEventArgs e)
        {
        }
        
        /// <summary>
        /// Implement this method to handle the entry of the 'Wait' state.
        /// </summary>
        protected virtual void OnWaitEntered()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the exit of the 'Wait' state.
        /// </summary>
        protected virtual void OnWaitExited()
        {
        }
        
        /// <summary>
        /// Implement this method to handle the transition below:<br/>
        /// InitialPurchaseOfA --&gt; Wait : Continue<br/>
        /// </summary>
        protected virtual void OnWaitEnteredFromContinueTrigger()
        {
        }
        
    }
}
