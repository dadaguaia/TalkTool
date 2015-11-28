﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Client;

namespace Client
{
    public class SocketTcp
    {
        public string adr;
        public int comNum;
        public string name;
        public TcpClient clientTcp;
        public NetworkStream stream;
        public OnlineList fatherWind;
        public ListFrom listFrom;
        public ChatForm chatForm;

        public SocketTcp(string Name, string Adr, int ComNum )
        {
            adr = Adr;
            name = Name;
            comNum = ComNum;
            clientTcp = new TcpClient();
            clientTcp.Connect(adr, comNum);
            stream = clientTcp.GetStream();
        }

        //接收数据
        public  void Receive()
        {
            int getSize;
            byte[] ReceiveBytes = new byte[1024];
            string ReceiveStr = null;
            while( true )
            {
                while (true)
                {
                    getSize = stream.Read(ReceiveBytes, 0, 1024);
                    if (getSize == 0)
                        break;
                    ReceiveStr = Encoding.BigEndianUnicode.GetString(ReceiveBytes, 0, getSize);
                    ReceiveStr.TrimEnd();
                    if(ReceiveStr.StartsWith("++-"))
                    {
                        ReceiveOnline(ReceiveStr);
                    }

                    if(ReceiveStr.StartsWith("+--"))
                    {
                        ReceiveChatNum(ReceiveStr);
                    }
                    if(ReceiveStr.StartsWith("---"))
                    {
                        ReceiveChatMsg(ReceiveStr);
                    }

                }
            }
        }

        //发送数据
        public static void Send(string str, NetworkStream stream)
        {
            byte[] SendBytes = new byte[1024];
                SendBytes = Encoding.BigEndianUnicode.GetBytes(str);
                stream.Write(SendBytes, 0, SendBytes.Length);
        }


        public void SendName()
        {            
            SocketTcp.Send("+++" + name,stream);
        }
        public void SendSerch()
        {
            SocketTcp.Send("++-",stream);
        }

        public void ReceiveOnline(string ReceiveStr)
        {
            string[] StrArray;
            ReceiveStr = ReceiveStr.Substring(3, ReceiveStr.Length - 3);
            StrArray = ReceiveStr.Split('*');
            foreach (string str in StrArray)
            {
                fatherWind.listBoxName.Items.Add(str);
            }
        }
        public void ChatConnect(string aim)
        {
            
            SocketTcp.Send("+--"+aim, stream);
            
        }
        public void ReceiveChatNum(string rec)
        {
            rec = rec.Substring(3, rec.Length - 3);
            string[] strArry;
            strArry = rec.Split('/');
            string aimStr = strArry[0];
            int aimNum = int.Parse(strArry[1]);
            ChatForm aimChat = null;
            foreach(var temp in listFrom.chatList)
            {
                if (temp.chat.aim == aimStr)
                    aimChat = temp;
            }
            aimChat.aimNum = aimNum;
            //chatForm.labAimNum.Text = chatForm.aimNum.ToString();    
            chatForm.labAimNum.Text = aimNum.ToString();
        }

        public void SendChat(string msg,ChatForm chatf)
        {
            string sendStr = "---";
            sendStr += chatf.aimNum.ToString();        
            sendStr += "/";            
            sendStr += msg;
            Send(sendStr, stream);
        }

        public void ReceiveChatMsg(string str)
        {
            string num ;
            string [] strArray;
            string rec;
            str = str.Substring(3, str.Length - 3);
            strArray = str.Split('/');
            num = strArray[0];
            rec = strArray[1];
            listFrom.richlist.Text += str+"\n";
            ChatForm aimForm = null ;
            foreach (var temp in listFrom.chatList)
            {
                if(temp.chat.aim == num )
                {
                    aimForm = temp;
                }
            }
            aimForm.ricTexReceive.Text += rec+"\n";
        }

    }
}
