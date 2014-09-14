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
using System.Threading;
using System.Windows;
using System.Runtime;
using System.IO;
using System.Diagnostics;
using System.Reflection;

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
        }
        /// <summary>
        /// vGamePadのスタートアップ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
#else
            var digitizer = Digitizer.GetDigitizer();
            if (!digitizer.Supported)
            {
                var dialog = new vGamePad.DialogWindow.DialogWindow(
                    vGamePad.Properties.Resources.ExceptionTitle, 
                    vGamePad.Properties.Resources.ExceptionMessage002);
                dialog.ShowDialog();
                Shutdown(-1);
            }
#endif
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ver.CompanyName + "\\" + ver.ProductName;
            Directory.CreateDirectory(profilePath);
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("vGamePad.JIT.Profile");

            Directory.CreateDirectory(UserDataPath);

            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                mutex = null;
                Shutdown(-1);
            }
            else
            {
                if (vGamePad.Properties.Settings.Default.IsUpgraded == false)
                {
                    // Upgradeを実行する
                    vGamePad.Properties.Settings.Default.Upgrade();
                    // 「Upgradeを実行した」という情報を設定する
                    vGamePad.Properties.Settings.Default.IsUpgraded = true;
                    // 現行バージョンの設定を保存する
                    vGamePad.Properties.Settings.Default.Save();
                }
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
        /// ハンドルされていない例外を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var dialog = new vGamePad.DialogWindow.DialogWindow(
                vGamePad.Properties.Resources.ExceptionTitle,
                e.Exception.Message + "\n" + vGamePad.Properties.Resources.ExceptionMessage001);
            dialog.ShowDialog();
            e.Handled = true;
            Shutdown(-1);
        }
        /// <summary>
        /// UIスレッド以外からの例外
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            var dialog = new vGamePad.DialogWindow.DialogWindow(
                vGamePad.Properties.Resources.ExceptionTitle,
                ex.Message + "\n" + vGamePad.Properties.Resources.ExceptionMessage001);
            dialog.ShowDialog();
            Shutdown(-1);
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
        /// <summary>
        /// アプリケーションのデータ保存用パスの取得
        /// </summary>
        public static string UserDataPath {
            get
            {
                var ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + ver.CompanyName + "\\" + ver.ProductName;
            }
        }
    }
}
