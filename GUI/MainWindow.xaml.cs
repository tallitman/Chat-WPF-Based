using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer;
using ILogger;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        private Logger logger;
        private ChatRoom chatroom;

        enum EnterType : int { loginClick = 1, register };//holds the option clicked by the user
        
        public MainWindow()
        {
            InitializeComponent();
            this.logger = Logger.Instance;                // initiates new logger and declares that startup was successful
            logger.logInfoMessage("The program was successfully started ");    
            this.chatroom = new ChatRoom(logger);
        }
        //login button event
        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            logger.logInfoMessage("Login button were clicked in main window");
            LoginRegisterWindow logRegWin = new LoginRegisterWindow((int)EnterType.loginClick, logger,chatroom);//open login window
            logRegWin.Show();
            Close();
        }
        //exit button event
        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            logger.logInfoMessage("Exit button were clicked in Main Window");
            //close Main window
            Close();
        }
        //register button event
        private void button_register_Click(object sender, RoutedEventArgs e)
        {
            logger.logInfoMessage("Register button were clicked in main window");
            LoginRegisterWindow logRegWin = new LoginRegisterWindow((int)EnterType.register, logger, chatroom);//open registration window
            logRegWin.Show();
            Close();
        
        }
    }
}
