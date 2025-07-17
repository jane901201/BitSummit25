using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using UnityEngine;

namespace Ghosts
{
    public class DirectionChangeTrigger : MonoBehaviour
    {
        //[SerializeField] private SwingSubDirection currentPosition = SwingSubDirection.None;
        
        
        static readonly Dictionary<SwingInfo, SwingInfo> ClockwiseMap = new()
        {
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.Up)]     = new(SwingDirection.Diagonal, SwingSubDirection.UpRight),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.Down)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.Left)]     = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.Right)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownRight),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.Up)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpRight),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.Down)]       = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.Left)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.Right)]       = new(SwingDirection.Diagonal, SwingSubDirection.DownRight),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.UpRight)] = new(SwingDirection.Vertical, SwingSubDirection.Right),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.DownLeft)] = new(SwingDirection.Horizontal, SwingSubDirection.Left),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.UpLeft)] = new(SwingDirection.Vertical, SwingSubDirection.Up),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.DownRight)] = new(SwingDirection.Horizontal, SwingSubDirection.Down),
        };

        static readonly Dictionary<SwingInfo, SwingInfo> CounterClockwiseMap = new()
        {
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.DownLeft)] = new(SwingDirection.Horizontal, SwingSubDirection.None),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.UpLeft)] = new(SwingDirection.Vertical, SwingSubDirection.None),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.DownRight)] = new(SwingDirection.Vertical, SwingSubDirection.None),
            [new SwingInfo(SwingDirection.Diagonal, SwingSubDirection.UpRight)] = new(SwingDirection.Horizontal, SwingSubDirection.None),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.None)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.None)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.None)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.None)]     = new(SwingDirection.Diagonal, SwingSubDirection.DownLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.None)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.None)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.None)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft),
            [new SwingInfo(SwingDirection.Vertical, SwingSubDirection.None)]       = new(SwingDirection.Diagonal, SwingSubDirection.UpLeft)
        };
        
        private void OnTriggerEnter(Collider other)
        {
            BossGhostPart bossGhostPart = other.GetComponent<BossGhostPart>();
            BossGhost bossGhost = bossGhostPart.BossGhost;
            
            if (bossGhost.CurrentRotationDirection == RotationDirection.Clockwise)
            {
                // if ((currentPosition == SwingSubDirection.UpRight || currentPosition == SwingSubDirection.DownLeft
                //                                                  || currentPosition == SwingSubDirection.UpLeft ||
                //                                                  currentPosition == SwingSubDirection.DownRight)
                //     && bossGhostPart.CurrentDirection.Base == SwingDirection.Diagonal)
                // {
                //     
                // }
                if (ClockwiseMap.TryGetValue(bossGhostPart.CurrentDirection, out var next))
                {
                    bossGhostPart.CurrentDirection = next;
                }
            }

            if (bossGhost.CurrentRotationDirection == RotationDirection.CounterClockwise)
            {
                // if (CounterClockwiseMap.TryGetValue(bossGhostPart.CurrentDirection, out var next))
                // {
                //     bossGhostPart.CurrentDirection = next;
                // }
            }
            
        }
        
    }
}