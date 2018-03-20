﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using MySql.Data.MySqlClient;

namespace HelloFormFlowBot
{


    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<ProfileForm> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(ProfileForm.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // Detect if this is a Message activity
            if (activity.Type == ActivityTypes.Message)
            {
                // Get any saved values
                StateClient sc = activity.GetStateClient();
                BotData userData = sc.BotState.GetPrivateConversationData(
                     activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));


                if (!boolProfileComplete)
                {
                    // Call our FormFlow by calling MakeRootDialog
                    await Conversation.SendAsync(activity, MakeRootDialog);
                }
                else
                {
                    var Name = userData.GetProperty<string>("Name");
                    var Email = userData.GetProperty<string>("Email");
                    var Gender = userData.GetProperty<string>("Sexe");
                    var Choice = userData.GetProperty<string>("Choice");
                    var Studie = userData.GetProperty<string>("Studie");
                    var Motivation = userData.GetProperty<string>("Motivation");
                    var WhatWeEarn = userData.GetProperty<string>("WhatWeEarn");
                    var centreInteret = userData.GetProperty<string>("centreInteret");

                    // Tell the user their profile is complete
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("Votre Reponse est enregistré\n\n");

                    // Create final reply
                    Activity replyMessage = activity.CreateReply(sb.ToString());
                    var conn = new DBConnect();

                    var insert = "INSERT INTO members VALUES (11, '" + Name + "', '" + Email + "', '" + WhatWeEarn + "', '" + Motivation + "',' " + centreInteret + "',' " + Gender + "',' " + Choice + "',' " + Studie + "', NULL, NULL)";
                    conn.Insert(insert);
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
            }
            else
            {
                // This was not a Message activity
                HandleSystemMessage(activity);
            }

            // Send response
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            //server = "212.1.212.234";
            server = "localhost";
            //database = "cseclubo_messanger_inscription";
            database = "s";
            //uid = "cseclubo_salah";
            uid = "root";
            //password = "[ sD=;fILrGuR= ]";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }

        }


        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Insert statement
        public void Insert(string query)
        {

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
        public bool Check()
        {
            if (this.OpenConnection() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}