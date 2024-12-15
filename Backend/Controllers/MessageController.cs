using System;
using AutoMapper;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Helpers;
using Backend.Models;
using Backend.Services.MessageService;
using Backend.Services.UsersService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
public class MessageController(IMessageService messageService,IUsersService userService , IMapper mapper ):BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO messagecreate)
    {
        var username=User.getUsernameFromToken();
        if(username==null) throw new Exception("user not identified");
        if(username.ToLower() == messagecreate.TargetUsername.ToLower())return BadRequest("do not message yourself");
        var sender =await userService.GetUserByUsernameAsync(username);
        var receiver =  await userService.GetUserByUsernameAsync(messagecreate.TargetUsername);
        if(sender == null || receiver==null)return BadRequest("can not send message");
        Message newMessage = new()
        {
            SourceUser=sender,
            SourceUserId = sender.Id,
            TargetUser=receiver,
            TargetUserId = receiver.Id,
            Content=messagecreate.Content
        };
        messageService.AddMessage(newMessage);
        if(await messageService.SaveChangeAsync())return Ok(mapper.Map<MessageDTO>(newMessage));
        else return BadRequest("can not send message");

    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesUser([FromQuery]MessageParams messageParams){
        messageParams.Username = User.getUsernameFromToken();
        var messages = await messageService.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messages);
        return messages;
    }
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesUser(string username){
        var currentUsername=User.getUsernameFromToken();
        if(currentUsername==null)return Unauthorized("need authorization");
        if(await userService.GetUserByUsernameAsync(username)==null)return BadRequest("can not find conversation");
        var messages= await messageService.GetMessageThread(currentUsername!,username);
        return Ok(messages);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMessage(int id){
        var username =User.getUsernameFromToken();
        var userId = User.getUserIdFromToken();
        var message =await messageService.GetMessage(id);
        if(message==null)return BadRequest("message not found");
        if(message.SourceUserId !=userId && message.TargetUserId !=userId) return Forbid("you can not touch that message");
        if(message.SourceUserId==userId)message.SourceDeleted=true;
        if (message.TargetUserId==userId)message.TargetDeleted=true;
        if(message is {TargetDeleted : true,SourceDeleted:true})messageService.DeleteMessage(message);
        if(await messageService.SaveChangeAsync()) return NoContent();
        return BadRequest("problem deletingthe message");

    }
}