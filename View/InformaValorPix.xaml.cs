﻿using System.Windows;
using VMIClientePix.View.Interfaces;

namespace VMIClientePix.View
{
    /// <summary>
    /// Interaction logic for InformaValorPix.xaml
    /// </summary>
    public partial class InformaValorPix : Window, ICloseable
    {
        public InformaValorPix()
        {
            InitializeComponent();
        }

        private void TelaInformarValorPix_Loaded(object sender, RoutedEventArgs e)
        {
            TxtValor.SelectAll();
        }
    }
}
