using System;

namespace Game.Logic.Actions
{
    public class CheckPVEGameStateAction : IAction
    {
        private long m_time;
        private bool m_isFinished;

        public CheckPVEGameStateAction(int delay)
        {
            this.m_time = TickHelper.GetTickCount() + (long)delay;
            this.m_isFinished = false;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (this.m_time > tick || game.GetWaitTimer() >= tick)
                return;
            if (game is PVEGame pveGame)
            {
                switch (pveGame.GameState)
                {
                    case eGameState.Inited:
                        pveGame.Prepare();
                        break;
                    case eGameState.Prepared:
                        pveGame.PrepareNewSession();
                        break;
                    case eGameState.Loading://3
                        if (pveGame.IsAllComplete())
                        {
                            pveGame.StartGame();
                            if (pveGame.OpenTryAgain)
                            {
                                pveGame.OpenTryAgain = false;
                            }
                            else
                            {
                                pveGame.PreSessionId++;
                            }
                            break;
                        }
                        else
                        {
                            game.SendLoading();
                            game.WaitTime(1000);
                        }
                        break;
                    case eGameState.GameStart://4
                        if (game.RoomType == eRoomType.FightLab)
                        {
                            if (game.CurrentActionCount <= 1)
                            {
                                pveGame.PrepareFightingLivings();
                                break;
                            }
                            break;
                        }
                        pveGame.PrepareNewGame();
                        break;
                    case eGameState.Playing:
                        if (pveGame.CurrentLiving != null && pveGame.CurrentLiving.IsAttacking || game.CurrentActionCount > 1)
                            break;
                        if(!pveGame.CanGameOver())
                        {
                            pveGame.NextTurn();
                            break;
                        }
                        else if(pveGame.IsLabyrinth() && pveGame.CanEnterGate)
                        {
                            pveGame.GameOverMovie();
                            break;
                        }
                        else if (!pveGame.ArenaBoss())
                        {
                            pveGame.GameOver();
                            break;
                        }
                        else
                        {
                            pveGame.GameOverArenaAll();
                            break;
                        }
                        #region OLD
                        /*if ((pveGame.CurrentLiving == null || !pveGame.CurrentLiving.IsAttacking) && game.CurrentActionCount <= 1)
                        {
                            if (pveGame.CanGameOver())
                            {
                                if (pveGame.IsLabyrinth() && pveGame.CanEnterGate)
                                {
                                    pveGame.GameOverMovie();
                                    break;
                                }
                                else if (pveGame.ArenaBoss())
                                {
                                    pveGame.GameOverArenaAll();
                                    break;
                                }
                                if (pveGame.CurrentActionCount <= 1)
                                {
                                    pveGame.GameOver();
                                    break;
                                }
                                break;
                            }
                            pveGame.NextTurn();
                            break;
                        }
                        break;*/
                    #endregion
                    case eGameState.PrepareGameOver:
                        if (pveGame.CurrentActionCount <= 1)
                        {
                            pveGame.GameOver();
                            break;
                        }
                        break;
                    case eGameState.GameOver://1
                        if(pveGame.HasNextSession())
                        {
                            pveGame.PrepareNewSession();
                            break;
                        }
                        else if(!pveGame.ArenaBoss())
                        {
                            pveGame.GameOverAllSession();
                            break;
                        }
                        else
                        {
                            pveGame.GameOverArenaAll();
                            break;
                        }
                    #region OLD
                    /*if (!pveGame.HasNextSession())
                    {
                        if (pveGame.ArenaBoss())
                        {
                            pveGame.GameOverArenaAll();
                        }
                        else
                        {
                            pveGame.GameOverAllSession();
                        }
                    }
                    pveGame.PrepareNewSession();
                    break;*/
                    #endregion
                    case eGameState.SessionPrepared://2
                        if (pveGame.CanStartNewSession())
                        {
                            pveGame.SetupStyle();
                            pveGame.StartLoading();
                            break;
                        }
                        game.WaitTime(1000);
                        break;
                    case eGameState.ALLSessionStopped:
                        #region
                        /*if (pveGame.PlayerCount != 0 && pveGame.WantTryAgain != 0 && !pveGame.IsWrong)
                        {
                            if (pveGame.WantTryAgain == 1)
                            {
                                pveGame.ShowDragonLairCard();
                                pveGame.PrepareNewSession();
                                break;
                            }
                            if (pveGame.WantTryAgain == 2)
                            {
                                if (pveGame.WantTryAgain == 2)
                                {
                                    --pveGame.SessionId;
                                    pveGame.PrepareNewSession();
                                    break;
                                }
                                game.WaitTime(1000);
                                break;
                            }
                            if (pveGame.IsWin)
                            {
                                pveGame.Stop();
                                break;
                            }
                            break;
                        }
                        pveGame.Stop();*/
                        #endregion
                        if (pveGame.PlayerCount == 0 || pveGame.WantTryAgain == 0)
                        {
                            pveGame.Stop();
                        }
                        else if (pveGame.WantTryAgain == 1)//back stage fail if player accept pay Money try agian
                        {
                            pveGame.RestoreSession();
                            pveGame.ShowDragonLairCard();
                            pveGame.PrepareNewSession();
                        }
                        else if (pveGame.WantTryAgain == 2)//show form ask player want try agian or not
                        {
                            if (!pveGame.MissionTryAgain)
                            {
                                pveGame.SendMissionTryAgain();
                                pveGame.MissionTryAgain = true;
                            }
                            if (!pveGame.ResponseMissionTryAgain())
                            {
                                pveGame.Stop();
                                //Console.WriteLine("pve.GameState.{0}", pve.GameState);
                            }
                        }
                        else if(pveGame.IsWin)
                        {
                            pveGame.Stop();
                            break;
                        }
                        game.WaitTime(1000);
                        break;
                }
            }
            this.m_isFinished = true;
        }

        public bool IsFinished(long tick) => this.m_isFinished;
    }
}
