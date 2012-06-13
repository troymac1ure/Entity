// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Authenticate.cs" company="">
//   
// </copyright>
// <summary>
//   Authenticate.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace entity
{
    // using System;
    // using System.Collections;
    // using System.ComponentModel;
    // using System.Drawing;
    // using System.IO;
    // using System.Windows.Forms;
    // using Fluent;
    // using Fluent.Toc;
    // <summary>
    /// Summary description for Authenticate2.
    /// </summary>
    // public class Authenticate
    // {
    // #region Fields
    // string currUser;
    // string currver;
    // ArrayList names = new ArrayList(0);
    // ArrayList passes = new ArrayList(0);
    // TocClient toc = new TocClient();
    // #endregion Fields
    // #region Constructors
    // public Authenticate()
    // {
    // names.Add("entitywatch01");
    // passes.Add("loser");
    // names.Add("entitywatch02");
    // passes.Add("loser");
    // names.Add("entitywatch03");
    // passes.Add("loser");
    // names.Add("icukunt");
    // passes.Add("halonazi");
    // names.Add("entitywatch");
    // passes.Add("l337");
    // names.Add("entitywatch2");
    // passes.Add("l337");
    // names.Add("entitywatch3");
    // passes.Add("l337");
    // }
    // #endregion Constructors
    // #region Methods
    // public bool authorize(string namex)
    // {
    // for (int x=0;x<names.Count;x++)
    // {
    // try
    // {
    // toc.SignOn((string)names[x],(string)passes[x]);
    // try
    // {
    // toc.Send(namex, "Version: " + currver + " Computer Name: " + System.Environment.MachineName + " - User Name:" + System.Environment.UserName + " - BetaTester:" + currUser, true);
    // return true;
    // }
    // catch
    // {
    // }
    // }
    // catch
    // {
    // }
    // }
    // return false;
    // }
    // public void run(string ver,string namex)
    // {
    // currver = ver;
    // currUser = namex;
    // bool i=authorize("pukeanddeadstuff");
    // bool f=authorize("thetyckoman");
    // bool ff=authorize("mjdpdu");
    // if (i==false&&f==false&&ff==false)
    // {
    // Application.Exit();
    // return;
    // }
    // toc.SignOff();
    // toc.StopListening();
    // //toc.StartListening();
    // //toc.Message+=new MessageEventHandler(toc_Message);
    // }
    // private void button1_Click(object sender, System.EventArgs e)
    // {
    // }
    // #endregion Methods
    // }
}