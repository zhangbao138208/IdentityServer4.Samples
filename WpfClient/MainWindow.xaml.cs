using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscoveryDocumentResponse _disco;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RequestAccessToken_ButtenClick(object sender, RoutedEventArgs e)
        {
            var userName = UserNameInput.Text;
            var password = PasswordInput.Password;

            var client = new HttpClient();
            _disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (_disco.IsError)
            {
                MessageBox.Show(_disco.Error);
                return;
            }
            var tokenRequest= await client.RequestPasswordTokenAsync(new PasswordTokenRequest { 
                Address=_disco.TokenEndpoint,
                ClientId= "wpf client",
                ClientSecret= "wpf client",
                Scope="api1 openid profile email phone address",
                Password=password,
                UserName=userName,
            });
            if (tokenRequest.IsError)
            {
                MessageBox.Show(tokenRequest.Error);
                return;
            }
            AccessTokenTestBlock.Text = tokenRequest.AccessToken;
        }

       
        private async void RequestApiResourceButton_Click(object sender, RoutedEventArgs e)
        {
            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Add("Accept","application/json");
            apiClient.SetBearerToken(AccessTokenTestBlock.Text);
            
            var response = await apiClient.GetAsync("http://localhost:5000/api/companies");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            RequestApiTestBlock.Text = content;
        }

        private async void RequestIdentityResourceButton_Click(object sender, RoutedEventArgs e)
        {
            var identityClient = new HttpClient();
            identityClient.SetBearerToken(AccessTokenTestBlock.Text);
            var response = await identityClient.GetAsync(_disco.UserInfoEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            RequestIdentityTestBlock.Text = content;
        }
    }
}
