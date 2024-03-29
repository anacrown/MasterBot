﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using BomberMan_SuperAI.Annotations;
using BomberMan_SuperAI.Controls;
using BotBase.Board;
using BotBase.BotInstance;
using BotBase.Interfaces;

namespace BomberMan_SuperAI.BattleSolver
{
    [Serializable]
    public class BomberSolver : ISolver, INotifyPropertyChanged
    {
        private int _size;
        private Board<CellBase> _board;
        private UIElement _control;
        private UIElement _debugControl;

        public BomberSolver() { }

        protected BomberSolver(SerializationInfo info, StreamingContext context) : this()
        {
            
        }

        public void Initialize()
        {
            
        }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            throw new NotImplementedException();
        }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, IDataProvider dataProvider, out string response)
        {
            throw new NotImplementedException();
        }

        public int Size
        {
            get => _size;
            set
            {
                if (value == _size) return;
                _size = value;
                OnPropertyChanged();
            }
        }

        public Board<CellBase> Board
        {
            get => _board;
            private set
            {
                if (Equals(value, _board)) return;
                _board = value;
                OnBoardChanged();
                OnPropertyChanged();
            }
        }

        public string Answer(Board<CellBase> board)
        {
            Board = board;

            /*
             
            Команд несколько: UP, DOWN, LEFT, RIGHT – приводят к движению героя в заданном направлении на 1 клетку; 
            ACT - оставить бомбу на месте героя. Команды движения можно комбинировать с командой ACT, разделяя их через запятую. 
            Порядок (LEFT, ACT) или (ACT, LEFT) - имеет значение, либо двигаемся влево и там ставим бомбу, либо ставим бомбу а затем тикаем влево. 
            Если игрок будет использовать только одну команду ACT, то бомба установится под героем без его перемещения на поле.
             
             */

            var move = new[]
            {
                "UP",
                "DOWN",
                "LEFT",
                "RIGHT",
            };

            var act = "ACT";

            var m = new Random().Next(4);
            var a = new Random().Next(2);
            var b = new Random().Next(2);

            var rsp = a == 1 ? b == 1 ? $"{act},{move[m]}" : $"{move[m]},{act}" : move[m];

            OnLogDataReceived(board.Frame, $"{m} {a} {b} {rsp}");

            return rsp;
        }

        public event EventHandler<Board<CellBase>> BoardChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void OnBoardChanged() => BoardChanged?.Invoke(this, Board);

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(DataFrame frame, string message) => LogDataReceived?.Invoke(this, new LogRecord(frame, message));

        public UIElement Control => _control ?? (_control = new BomberSolverControl(this));
        public UIElement DebugControl => _debugControl ?? (_debugControl = new BomberSolverDebugControl(this));

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }

    public enum Element
    {
        /// This is your Bomberman
        BOMBERMAN, //('☺'),             // так выглядит мой бомбер
        BOMB_BOMBERMAN, //('☻'),        // так выглядит мой бомбер, если он сидит на бомбе
        DEAD_BOMBERMAN, //('Ѡ'),        // ойкс! твой бомбер умер. Не волнуйся, он появится
        // через секунду где-нибудь на поле, но вполне
        // вероятно за это ты получишь штрафные очки.

        /// this is other players Bombermans
        OTHER_BOMBERMAN, //('♥'),       // а так выглядит бомбер противника
        OTHER_BOMB_BOMBERMAN, //('♠'),  // так, если бомбер противника сидит на бомбе
        OTHER_DEAD_BOMBERMAN, //('♣'),  // так, если бомбер противника подорвался.
        // если это ты его подорвал - ты получишь бонусные очки.

        /// the bombs
        BOMB_TIMER_5, //('5'),          // после того как бомбер поставит бомбу таймер вкючится (всего 5 тиков)
        BOMB_TIMER_4, //('4'),          // эта бомба взорвется через 4 тика
        BOMB_TIMER_3, //('3'),          // эта - через 3
        BOMB_TIMER_2, //('2'),          // два
        BOMB_TIMER_1, //('1'),          // один
        BOOM, //('҉'),                  // Бам! Это то, как бомба взрывается. При этом все, что может быть разрушено - разрушится

        /// walls
        WALL, //('☼'),                  // неразрушаемые стены - им взрывы бомб не страшны
        DESTROYABLE_WALL, //('#'),      // а эта стенка может быть разрушена
        DESTROYED_WALL, //('H'),        // это как разрушенная стенка выглядит, она пропадет в следующую секунду
        // если это ты сделал - ты получишь бонусные очки

        /// meatchoppers
        MEAT_CHOPPER, //('&'),          // этот малый бегает по полю в произвольном порядке

        // если он дотроентся до бомбера - тот умрет
        // лучше бы тебе учничтожить этот кусок.... мяса, за это ты получишь бонусные очки
        DEAD_MEAT_CHOPPER, //('x'),     // это взровравшийся митчопер

        /// a void
        NONE //(' ');                 // свободная ячейка, куда ты можешь направить бомбера
    }

    public static class CellExtention
    {
        private static Dictionary<string, Element> _elements = new Dictionary<string, Element>()
        {
            {"☺", Element.BOMBERMAN },
            {"☻", Element.BOMB_BOMBERMAN },
            {"Ѡ", Element.DEAD_BOMBERMAN },
            {"♥", Element.OTHER_BOMBERMAN },
            {"♠", Element.OTHER_BOMB_BOMBERMAN },
            {"♣", Element.OTHER_DEAD_BOMBERMAN },
            {"5", Element.BOMB_TIMER_5 },
            {"4", Element.BOMB_TIMER_4 },
            {"3", Element.BOMB_TIMER_3 },
            {"2", Element.BOMB_TIMER_2 },
            {"1", Element.BOMB_TIMER_1 },
            {"'", Element.BOOM },
            {"☼", Element.WALL },
            {"#", Element.DESTROYABLE_WALL },
            {"H", Element.DESTROYED_WALL },
            {"&", Element.MEAT_CHOPPER },
            {"x", Element.DEAD_MEAT_CHOPPER },
            {" ", Element.NONE }
        };

        public static Element GetElement(this CellBase cell)
        {
            return _elements[cell.C];
        }
    }
}
