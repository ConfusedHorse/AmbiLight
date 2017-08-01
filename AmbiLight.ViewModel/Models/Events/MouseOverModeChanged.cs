using System;
using System.Windows;
using AmbiLight.CrossCutting.Enums;
using AmbiLight.ViewModel.Models.Modes;

namespace AmbiLight.ViewModel.Models.Events
{
    public delegate void MouseOverModeChanged(object sender, MouseOverModeArgs color);

    public class MouseOverModeArgs : EventArgs
    {
        public MouseOverModeArgs(MouseOver state)
        {
            State = state;
        }

        public MouseOverModeArgs(IAmbiLightMode mode, MouseOver state, Point position)
        {
            Mode = mode;
            State = state;
            PositionOfMode = position;
        }

        public Point PositionOfMode { get; set; }

        public IAmbiLightMode Mode { get; set; }

        public MouseOver State { get; set; }
    }
}