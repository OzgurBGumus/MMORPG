// We implemented a chat system that works directly with UNET. The chat supports
// different channels that can be used to communicate with other players:
//
// - **Local Chat:** by default, all messages that don't start with a **/** are
// addressed to the local chat. If one player writes a local message, then all
// players around him _(all observers)_ will be able to see the message.
// - **Whisper Chat:** a player can write a private message to another player by
// using the **/ name message** format.
// - **Guild Chat:** we implemented guild chat support with the **/g message**
// - **Info Chat:** the info chat can be used by the server to notify all
// players about important news. The clients won't be able to write any info
// messages.
//
// _Note: the channel names, colors and commands can be edited in the Inspector_
using System;
using UnityEngine;
using Mirror;
using System.Text.RegularExpressions;
using System.Collections.Generic;



[RequireComponent(typeof(PlayerGuild))]
[RequireComponent(typeof(PlayerParty))]
[DisallowMultipleComponent]
public class PlayerPrivateChat : NetworkBehaviour
{
    [Header("Components")] // to be assigned in inspector
    public PlayerGuild guild;
    public PlayerParty party;

    [Header("Channels")]
    public ChannelInfo whisperChannel = new ChannelInfo("/w", "(TO)", "(FROM)", null);

    [Header("Other")]
    public int maxLength = 70;

    [Header("Events")]
    public UnityEventString onSubmit;

    public override void OnStartLocalPlayer()
    {
        // test messages
        UIChat.singleton.AddMessage(new ChatMessage("Someone", whisperChannel.identifierIn, "Are you there?", "/w Someone ",  whisperChannel.textPrefab));
    }

}
