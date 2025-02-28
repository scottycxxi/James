using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace James
{
//1 - in order, prior verse context, 100% help
//2 - in order, prior verse context, 60% help
//3 - in order, prior verse context, 30% help
//4 - in order, prior verse context, 0% help, (no word lengths)
//5 - random, prior and following verse context, 60% help
//6 - random, prior and following verse context, 30% help
//7 - random, prior and following verse context, 0% help(no word lengths)
//8 - type the whole thing, hit enter after each verse, ball out
//9 - random, no context, ball out
    enum GameModeType
    {
        ordered100Help = 1,
        ordered60Help = 2,
        ordered30Help = 3,
        ordered0Help = 4,
        random60Help = 5,
        random30Help = 6,
        random0Help = 7,
        orderedNoContext = 8,
        randomNoContext = 9
    }

    internal class GameMode
    {
        private GameModeType gameModeType;
        private bool inOrder;
        private int percentHelp;
        private bool suppressMistakesTilTheEnd;
        private bool showContext;

        public GameMode (GameModeType gameModeType)
        {
            this.gameModeType = gameModeType;
            //drewc change to table 
            switch (gameModeType)
            {
                //in order
                case GameModeType.ordered100Help:
                    inOrder = true;
                    percentHelp = 100;
                    suppressMistakesTilTheEnd = false;
                    showContext = false;
                    break;
                case GameModeType.ordered60Help:
                    inOrder = true;
                    percentHelp = 60;
                    suppressMistakesTilTheEnd = false;
                    showContext = false;
                    break;
                case GameModeType.ordered30Help:
                    inOrder = true;
                    percentHelp = 30;
                    suppressMistakesTilTheEnd = false;
                    showContext = false;
                    break;
                case GameModeType.ordered0Help:
                    inOrder = true;
                    percentHelp = 0;
                    suppressMistakesTilTheEnd = false;
                    showContext = false;
                    break;
                //random order
                case GameModeType.random60Help:
                    inOrder = false;
                    percentHelp = 60;
                    suppressMistakesTilTheEnd = false;
                    showContext = true;
                    break;
                case GameModeType.random30Help:
                    inOrder = false;
                    percentHelp = 30;
                    suppressMistakesTilTheEnd = false;
                    showContext = true;
                    break;
                case GameModeType.random0Help:
                    inOrder = false;
                    percentHelp = 0;
                    suppressMistakesTilTheEnd = false;
                    showContext = true;
                    break;
                //no context
                case GameModeType.orderedNoContext:
                    inOrder = true;
                    percentHelp = 0;
                    suppressMistakesTilTheEnd = true;
                    showContext = false;
                    break;
                case GameModeType.randomNoContext:
                    inOrder = false;
                    percentHelp = 0;
                    suppressMistakesTilTheEnd = true;
                    showContext = false;
                    break;

                default:
                    throw new Exception ("unhandled game mode: " + gameModeType);
            }
        }

        public GameModeType GameModeType { get { return gameModeType; } } 
        
        public bool InOrder { get { return inOrder; } }   

        public int PercentHelp { get { return percentHelp; } }

        public bool SuppressMistakesTilTheEnd { get {  return suppressMistakesTilTheEnd; } }

        public bool ShowContext { get { return showContext; } set { showContext = value; } }
    }
}
