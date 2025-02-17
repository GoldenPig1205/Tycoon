﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Mirror;
using MultiBroadcast.API;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using UnityEngine;
using static Tycoon.Core.Variables.Base;
using static Tycoon.Core.Functions.Base;

namespace Tycoon.Core.Commands.ClientCommands.basicfeatures
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChangeEmotion : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            try
            {
                if (EmotionCooldowns.Contains(player))
                {
                    response = "너무 빠른 간격으로 입력을 보내고 있습니다!";
                    return false;
                }
                else if (player.IsHuman)
                {
                    if (int.TryParse(arguments.At(0), out int num))
                    {
                        try
                        {
                            player.Emotion = (EmotionPresetType)(num - 1);

                            response = $"감정을 성공적으로 변경했습니다.";
                            return true;
                        }
                        catch
                        {
                            response = $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", EnumToList<EmotionPresetType>())}";
                            return false;
                        }
                    }
                    else
                    {
                        response = $"1~7번 사이에서 입력해주세요.\n\n{string.Join("\n", EnumToList<EmotionPresetType>())}";
                        return false;
                    }
                }
                else
                {
                    response = "인간만 사용 가능한 명령어입니다.";
                    return false;
                }
            }
            catch
            {
                player.Emotion = (EmotionPresetType)UnityEngine.Random.Range(0, 7);

                response = $"감정을 성공적으로 변경했습니다.";
                return true;
            }
        }

        public string Command { get; } = "감정";

        public string[] Aliases { get; } = { "감정표현" };

        public string Description { get; } = "[Tycoon] 감정 표현을 더 쉽게 하세요.";

        public bool SanitizeResponse { get; } = true;
    }
}
