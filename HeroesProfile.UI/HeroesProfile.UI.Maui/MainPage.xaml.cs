using System;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.BackgroundServices;
using HeroesProfile.Core.CQRS.Commands;

using MediatR;

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace HeroesProfile.UI.Maui
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage, IPage
    {    

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
