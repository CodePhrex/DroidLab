using System.Windows;
using System.Windows.Threading;
namespace DroidLab.Desktop;
public partial class App : Application {
 protected override void OnStartup(StartupEventArgs e){
 base.OnStartup(e); ShutdownMode=ShutdownMode.OnExplicitShutdown;
 var splash=new StartupSplash(); splash.Show();
 Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(()=>{
 try{var main=new MainWindow(); MainWindow=main; main.Show(); ShutdownMode=ShutdownMode.OnMainWindowClose;}
 catch(Exception ex){MessageBox.Show("Droid Lab could not initialize: "+ex.Message,"Droid Lab",MessageBoxButton.OK,MessageBoxImage.Error);Shutdown(1);}
 finally{splash.Close();}
 }));
 }
}