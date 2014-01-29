using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.AI.MaintenanceMode.Engines;
using TacticsGame.World;

namespace TacticsGame.Managers
{
    public class UnitActionManager
    {
        private IActionResultEngine resultEngine;

        private IDecisionEngine decisionEngine;

        private IActionResultTextManager textManager;

        private IPreferenceEngine preferenceEngine;

        private ITownMarketManager townMarketManager;

        public UnitActionManager()
        {
            this.preferenceEngine = new EducatedPreferenceEngine();
            this.resultEngine = new EducatedResultEngine();
            this.textManager = new ActionResultTextManager();
            this.townMarketManager = new TownMarketManager(this.preferenceEngine);
            this.decisionEngine = new EducatedDecisionEngine(this.preferenceEngine, this.townMarketManager);
        }

        public IActionResultEngine ResultEngine
        {
            get { return resultEngine; }
        }

        public IActionResultTextManager ResultTextManager
        {
            get { return textManager; }
        }

        public IDecisionEngine DecisionEngine
        {
            get { return decisionEngine; }
        }

        public IPreferenceEngine PreferenceEngine
        {
            get { return preferenceEngine; }
        }
    }
}
