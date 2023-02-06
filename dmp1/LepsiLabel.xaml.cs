﻿using System;
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

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro LepsiLabel.xaml
    /// </summary>
    public partial class LepsiLabel : UserControl
    {
        public static readonly DependencyProperty TextKZobrazeniProperty = DependencyProperty.Register(
        "TextKZobrazeni", typeof(string),
        typeof(LepsiLabel)
        );

        public string TextKZobrazeni
        {
            get => (string)GetValue(TextKZobrazeniProperty);
            set => SetValue(TextKZobrazeniProperty, value);
        }

        public static readonly DependencyProperty TucneProperty = DependencyProperty.Register(
        "Tucne", typeof(FontWeight),
        typeof(LepsiLabel)
        );

        public FontWeight Tucne
        {
            get => (FontWeight)GetValue(TextKZobrazeniProperty);
            set => SetValue(TextKZobrazeniProperty, value);
        }
        public LepsiLabel()
        {
            InitializeComponent();
        }
    }
}
