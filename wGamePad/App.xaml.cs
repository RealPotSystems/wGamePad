//------------------------------------------------------------
//
//  仮想ゲームパッド vGamePad
//  © 2014,RealPotSystems(TAKUBON). All rights reserverd.
//
//  Workfile : App.xaml.cs
//  Author   : TAKUYA MANABE(manataku@me.com)
//
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Runtime;

namespace vGamePad
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public sealed partial class App : Application,IDisposable
    {
        /// <summary>
        /// このプログラムのグリッド単位
        /// </summary>
        public static readonly int GRID = 16;
        /// <summary>
        /// このプログラムの動作モード
        /// </summary>
        public static bool MaintenanceMode = false;
        /// <summary>
        /// 多重起動抑止のミューテックス
        /// </summary>
        private Mutex mutex = new Mutex(false, "vGamePad");
        /// <summary>
        /// vGamePadのメインウィンドウ
        /// </summary>
        private MainWindow main = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App()
        {
            ProfileOptimization.SetProfileRoot(Environment.CurrentDirectory);
            ProfileOptimization.StartProfile("vGamePad.JIT.Profile");
        }
        /// <summary>
        /// vGamePadのスタートアップ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                mutex = null;
                Shutdown(-1);
            }
            else
            {
                main = new MainWindow();
                main.Show();
            }
        }
        /// <summary>
        /// vGamePadの終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //アプリケーションの設定を保存する
            vGamePad.Properties.Settings.Default.Save();
            Dispose();
        }
        /// <summary>
        /// mutexの解放漏れを防ぐためDisposeを実装
        /// </summary>
        public void Dispose()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex = null;
            }
        }
    }
}
