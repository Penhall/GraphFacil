using Dashboard;
using Dashboard.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using Windows.UI.Text;
using static Microsoft.ML.Data.SchemaDefinition;
using static System.Net.Mime.MediaTypeNames;

< !--Dashboard / MainWindow.xaml-- >
< Window x: Class = "Dashboard.MainWindow"
        xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns: d = "http://schemas.microsoft.com/expression/blend/2008"
        xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns: md = "http://materialdesigninxaml.net/winfx/xaml/themes"
        xmlns: vm = "clr-namespace:Dashboard.ViewModel"
        mc: Ignorable = "d"
        Width = "1400"
        Height = "900"
        WindowStartupLocation = "CenterScreen"
        WindowStyle = "None"
        Background = "#FF2C3E50"
        ResizeMode = "CanResize" >

    < Window.DataContext >
        < vm:MainWindowViewModel />
    </ Window.DataContext >

    < Window.Resources >
        < ResourceDictionary >
            < ResourceDictionary.MergedDictionaries >
                < ResourceDictionary Source = "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Button.xaml" />
                < ResourceDictionary Source = "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Card.xaml" />
                < ResourceDictionary Source = "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml" />
            </ ResourceDictionary.MergedDictionaries >

            < !--Conversores-- >
            < BooleanToVisibilityConverter x: Key = "BooleanToVisibilityConverter" />

            < !--Estilos Personalizados-- >
            < Style x: Key = "MenuButton" TargetType = "Button" BasedOn = "{StaticResource MaterialDesignRaisedButton}" >
                < Setter Property = "Background" Value = "#FF34495E" />
                < Setter Property = "Foreground" Value = "White" />
                < Setter Property = "BorderBrush" Value = "#FF5D6D7E" />
                < Setter Property = "BorderThickness" Value = "1" />
                < Setter Property = "Padding" Value = "15,8" />
                < Setter Property = "Margin" Value = "5" />
                < Setter Property = "FontSize" Value = "12" />
                < Setter Property = "FontWeight" Value = "SemiBold" />
                < Setter Property = "Height" Value = "45" />
                < Setter Property = "HorizontalAlignment" Value = "Stretch" />
                < Setter Property = "Effect" >
                    < Setter.Value >
                        < DropShadowEffect Color = "Black" Direction = "320" ShadowDepth = "2" BlurRadius = "4" Opacity = "0.3" />
                    </ Setter.Value >
                </ Setter >
                < Style.Triggers >
                    < Trigger Property = "IsMouseOver" Value = "True" >
                        < Setter Property = "Background" Value = "#FF3B82F6" />
                        < Setter Property = "BorderBrush" Value = "#FF60A5FA" />
                    </ Trigger >
                    < Trigger Property = "IsPressed" Value = "True" >
                        < Setter Property = "Background" Value = "#FF1E40AF" />
                    </ Trigger >
                </ Style.Triggers >
            </ Style >

            < Style x: Key = "MetronomoCard" TargetType = "Border" >
                < Setter Property = "Background" Value = "White" />
                < Setter Property = "CornerRadius" Value = "8" />
                < Setter Property = "Padding" Value = "15" />
                < Setter Property = "Margin" Value = "5" />
                < Setter Property = "Effect" >
                    < Setter.Value >
                        < DropShadowEffect Color = "Black" Direction = "320" ShadowDepth = "3" BlurRadius = "6" Opacity = "0.2" />
                    </ Setter.Value >
                </ Setter >
            </ Style >

            < Style x: Key = "StatusBar" TargetType = "Border" >
                < Setter Property = "Background" Value = "#FF1A252F" />
                < Setter Property = "Padding" Value = "10,5" />
                < Setter Property = "BorderBrush" Value = "#FF34495E" />
                < Setter Property = "BorderThickness" Value = "0,1,0,0" />
            </ Style >

            < !--Data Templates-- >
            < DataTemplate x: Key = "MetronomoTemplate" >
                < Border Style = "{StaticResource MetronomoCard}"
                        MouseLeftButtonDown = "Border_MouseLeftButtonDown" >
                    < Grid >
                        < Grid.RowDefinitions >
                            < RowDefinition Height = "Auto" />
                            < RowDefinition Height = "Auto" />
                            < RowDefinition Height = "Auto" />
                            < RowDefinition Height = "*" />
                        </ Grid.RowDefinitions >

                        < !--Número da Dezena -->
                        <TextBlock Grid.Row="0" 
                                   Text="{Binding Numero, StringFormat='D2'}" 
                                   FontSize="24" 
                                   FontWeight="Bold" 
                                   HorizontalAlignment="Center"
                                   Foreground="#FF2C3E50"/>

                        <!-- Probabilidade -->
                        <TextBlock Grid.Row="1" 
                                   Text="{Binding ProbabilidadeAtual, StringFormat='P1'}" 
                                   FontSize="16" 
                                   FontWeight="SemiBold" 
                                   HorizontalAlignment="Center"
                                   Foreground="#FF3B82F6"
                                   Margin="0,5"/>

                        <!-- Barra de Probabilidade -->
                        <ProgressBar Grid.Row="2" 
                                     Value="{Binding ProbabilidadeAtual}" 
                                     Maximum="1" 
                                     Height="8" 
                                     Background="#FFECF0F1"
                                     Foreground="#FF27AE60"
                                     Margin="0,5"/>

                        <!-- Informações Adicionais -->
                        <StackPanel Grid.Row="3" Margin="0,10,0,0">
                            <TextBlock Text="{Binding TipoMetronomo}" 
                                       FontSize="10" 
                                       Foreground="#FF7F8C8D" 
                                       HorizontalAlignment="Center"/>
                            <TextBlock Text="{Binding CicloMedio, StringFormat='Ciclo: {0:F1}'}" 
                                       FontSize="10" 
                                       Foreground="#FF7F8C8D" 
                                       HorizontalAlignment="Center"/>
                            <TextBlock Text="{Binding IntervalAtual, StringFormat='Há {0} concursos'}" 
                                       FontSize="10" 
                                       Foreground="#FF7F8C8D" 
                                       HorizontalAlignment="Center"/>
                        </StackPanel>

                        <!-- Indicador de Fase Ótima -->
                        <Ellipse Grid.Row="0" 
                                 Width="12" 
                                 Height="12" 
                                 HorizontalAlignment="Right" 
                                 VerticalAlignment="Top" 
                                 Margin="0,-5,-5,0">
                            <Ellipse.Fill>
                                <SolidColorBrush Color="{Binding EmFaseOtima, Converter={StaticResource BoolToColorConverter}}"/>
                            </Ellipse.Fill>
                        </Ellipse>
                    </Grid>
                </Border>
            </DataTemplate>
        </ResourceDictionary>
    </Window.Resources>

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="250"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="300"/>
        </Grid.ColumnDefinitions>

        <Grid.RowDefinitions>
            <RowDefinition Height="40"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="30"/>
        </Grid.RowDefinitions>

        <!-- Barra de Título -->
        <Border Grid.Column="0" Grid.ColumnSpan="3" Grid.Row="0" 
                Background="#FF1A252F" 
                MouseLeftButtonDown="TitleBar_MouseLeftButtonDown">
            <Grid>
                <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" VerticalAlignment="Center" Margin="15,0">
                    <md:PackIcon Kind = "Analytics" Foreground="#FF3B82F6" VerticalAlignment="Center" Margin="0,0,10,0"/>
                    <TextBlock Text="Sistema de Metrônomos Lotofácil v2.0" 
                               Foreground="White" 
                               FontWeight="SemiBold" 
                               VerticalAlignment="Center"/>
                </StackPanel>
                
                <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" VerticalAlignment="Center" Margin="15,0">
                    <Button x:Name = "BtnMinimize"
                            Content = "−"
                            Width = "30"
                            Height = "25"
                            Background = "Transparent"
                            Foreground = "White"
                            BorderThickness = "0"
                            Click = "BtnMinimize_Click" />
                    < Button x: Name = "BtnClose"
                            Content = "×"
                            Width = "30"
                            Height = "25"
                            Background = "Transparent"
                            Foreground = "White"
                            BorderThickness = "0"
                            Click = "BtnClose_Click"
                            FontSize = "16" />
                </ StackPanel >
            </ Grid >
        </ Border >

        < !--Painel Esquerdo - Controles Principais-- >
        < Border Grid.Column = "0" Grid.Row = "1"
                Background = "#FF34495E"
                BorderBrush = "#FF2C3E50"
                BorderThickness = "0,0,1,0" >
            < ScrollViewer VerticalScrollBarVisibility = "Auto" >
                < StackPanel Margin = "10" >


                    < !--Seção Metrônomos-- >
                    < TextBlock Text = "🎵 METRÔNOMOS"
                               Foreground = "White"
                               FontSize = "14"
                               FontWeight = "Bold"
                               Margin = "10,0,0,10" />


                    < Button Style = "{StaticResource MenuButton}"
                            Command = "{Binding IniciarMetronomosCommand}"
                            IsEnabled = "{Binding IsProcessing, Converter={StaticResource InverseBooleanConverter}}" >
                        < StackPanel Orientation = "Horizontal" >
                            < md:PackIcon Kind = "PlayCircle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Iniciar Metrônomos"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding GerarPalpiteCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "AutoFix" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Gerar Palpite"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding ValidarModeloCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "CheckCircle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Validar Modelo"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding CompararEstrategiasCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "CompareHorizontal" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Comparar Estratégias"/>
                        </StackPanel>
                    </Button>

                    <Separator Margin="0,15" Background="#FF5D6D7E"/>

                    <!-- Seção Configurações -->
                    <TextBlock Text="⚙️ CONFIGURAÇÕES" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="10,0,0,10"/>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding AlterarConcursoAlvoCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Target" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Alterar Concurso"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding ConfigurarTreinamentoCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Cog" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Config. Treino"/>
                        </StackPanel>
                    </Button>

                    <Separator Margin="0,15" Background="#FF5D6D7E"/>

                    <!-- Seção Diagnóstico -->
                    <TextBlock Text="🔧 DIAGNÓSTICO" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="10,0,0,10"/>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding DiagnosticarMetronomosCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Stethoscope" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Diagnosticar"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding ForcarRecalculoMetronomosCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Refresh" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Recalcular"/>
                        </StackPanel>
                    </Button>

                    <Separator Margin="0,15" Background="#FF5D6D7E"/>

                    <!-- Seção Estudos -->
                    <TextBlock Text="📊 ESTUDOS" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="10,0,0,10"/>

                    <Button Style="{StaticResource MenuButton}" Command="{Binding PrimeiroCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Numeric1Circle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Estudo 1"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" Command="{Binding SegundoCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Numeric2Circle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Estudo 2"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" Command="{Binding TerceiroCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Numeric3Circle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Estudo 3"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" Command="{Binding QuartoCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "Numeric4Circle" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Estudo 4"/>
                        </StackPanel>
                    </Button>

                    <Separator Margin="0,15" Background="#FF5D6D7E"/>

                    <!-- Seção Ferramentas -->
                    <TextBlock Text="🛠️ FERRAMENTAS" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="10,0,0,10"/>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding SalvarResultadosCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "ContentSave" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Salvar Relatório"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding AbrirValidacaoCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "ChartLine" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Análise ML"/>
                        </StackPanel>
                    </Button>

                    <Button Style="{StaticResource MenuButton}" 
                            Command="{Binding TerminarProgramaCommand}">
                        <StackPanel Orientation="Horizontal">
                            <md:PackIcon Kind = "ExitToApp" VerticalAlignment="Center" Margin="0,0,8,0"/>
                            <TextBlock Text="Sair"/>
                        </StackPanel>
                    </Button>

                </StackPanel>
            </ScrollViewer>
        </Border>

        <!-- Área Central - Visualização dos Metrônomos -->
        <Border Grid.Column="1" Grid.Row="1" 
                Background="#FFECF0F1" 
                Padding="20">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <!-- Header da Área Central -->
                <Grid Grid.Row="0" Margin="0,0,0,20">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="Auto"/>
                    </Grid.ColumnDefinitions>

                    <StackPanel Grid.Column="0">
                        <TextBlock Text="Metrônomos Individuais" 
                                   FontSize="24" 
                                   FontWeight="Bold" 
                                   Foreground="#FF2C3E50"/>
                        <TextBlock Text="{Binding TextoConcurso}" 
                                   FontSize="14" 
                                   Foreground="#FF7F8C8D" 
                                   Margin="0,5"/>
                    </StackPanel>

                    <StackPanel Grid.Column="1" Orientation="Horizontal">
                        <ToggleButton x:Name = "ToggleView"
                                      IsChecked = "{Binding MostrarMetronomos}"
                                      Content = "{md:PackIcon ViewGrid}"
                                      Style = "{StaticResource MaterialDesignActionToggleButton}"
                                      ToolTip = "Alternar Visualização"
                                      Margin = "5" />
                    </ StackPanel >
                </ Grid >

                < !--Último Palpite-- >
                < Border Grid.Row = "1"
                        Background = "#FF3B82F6"
                        CornerRadius = "8"
                        Padding = "15"
                        Margin = "0,0,0,20"
                        Visibility = "{Binding UltimoPalpite, Converter={StaticResource StringToVisibilityConverter}}" >
                    < Grid >
                        < Grid.ColumnDefinitions >
                            < ColumnDefinition Width = "Auto" />
                            < ColumnDefinition Width = "*" />
                        </ Grid.ColumnDefinitions >

                        < md:PackIcon Grid.Column = "0"
                                     Kind = "TrophyAward"
                                     Foreground = "White"
                                     VerticalAlignment = "Center"
                                     Margin = "0,0,15,0"
                                     Width = "24"
                                     Height = "24" />

                        < StackPanel Grid.Column = "1" >
                            < TextBlock Text = "ÚLTIMO PALPITE GERADO"
                                       Foreground = "White"
                                       FontWeight = "Bold"
                                       FontSize = "12" />
                            < TextBlock Text = "{Binding UltimoPalpite}"
                                       Foreground = "White"
                                       FontSize = "16"
                                       FontWeight = "SemiBold"
                                       Margin = "0,5" />
                        </ StackPanel >
                    </ Grid >
                </ Border >

                < !--Grid de Metrônomos -->
                <ScrollViewer Grid.Row="2" 
                              VerticalScrollBarVisibility="Auto" 
                              Visibility="{Binding MostrarMetronomos, Converter={StaticResource BooleanToVisibilityConverter}}">
                    <ItemsControl ItemsSource="{Binding Metronomos}" 
                                  ItemTemplate="{StaticResource MetronomoTemplate}">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <UniformGrid Columns="5" />
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                    </ItemsControl>
                </ScrollViewer>

            </Grid>
        </Border>

        <!-- Painel Direito - Informações e Status -->
        <Border Grid.Column="2" Grid.Row="1" 
                Background="#FF34495E" 
                BorderBrush="#FF2C3E50" 
                BorderThickness="1,0,0,0">
            <ScrollViewer VerticalScrollBarVisibility="Auto">
                <StackPanel Margin="15">

                    <!-- Status do Sistema -->
                    <TextBlock Text="📊 STATUS DO SISTEMA" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="0,0,0,15"/>

                    <Border Background="#FF2C3E50" 
                            CornerRadius="5" 
                            Padding="10" 
                            Margin="0,0,0,15">
                        <StackPanel>
                            <TextBlock Text="Engine:" 
                                       Foreground="#FF95A5A6" 
                                       FontSize="10"/>
                            <TextBlock Text="{Binding StatusEngine}" 
                                       Foreground="White" 
                                       FontSize="12" 
                                       TextWrapping="Wrap"/>
                            
                            <TextBlock Text="Confiança:" 
                                       Foreground="#FF95A5A6" 
                                       FontSize="10" 
                                       Margin="0,10,0,0"/>
                            <TextBlock Text="{Binding ConfiancaAtual, StringFormat='P1'}" 
                                       Foreground="#FF27AE60" 
                                       FontSize="16" 
                                       FontWeight="Bold"/>

                            <TextBlock Text="Processando:" 
                                       Foreground="#FF95A5A6" 
                                       FontSize="10" 
                                       Margin="0,10,0,0"/>
                            <TextBlock Text="{Binding IsProcessing}" 
                                       Foreground="White" 
                                       FontSize="12"/>
                        </StackPanel>
                    </Border>

                    <!-- Estatísticas Rápidas -->
                    <TextBlock Text="📈 ESTATÍSTICAS" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="0,0,0,15"/>

                    <ItemsControl ItemsSource="{Binding EstatisticasPorTipo}">
                        <ItemsControl.ItemTemplate>
                            <DataTemplate>
                                <Border Background="#FF2C3E50" 
                                        CornerRadius="3" 
                                        Padding="8" 
                                        Margin="0,2">
                                    <Grid>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="*"/>
                                            <ColumnDefinition Width="Auto"/>
                                        </Grid.ColumnDefinitions>
                                        
                                        <TextBlock Grid.Column="0" 
                                                   Text="{Binding Key}" 
                                                   Foreground="White" 
                                                   FontSize="11"/>
                                        
                                        <TextBlock Grid.Column="1" 
                                                   Text="{Binding Value}" 
                                                   Foreground="#FF3B82F6" 
                                                   FontWeight="Bold" 
                                                   FontSize="11"/>
                                    </Grid>
                                </Border>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>

                    <!-- Relatório Geral -->
                    <TextBlock Text="📋 RELATÓRIO" 
                               Foreground="White" 
                               FontSize="14" 
                               FontWeight="Bold" 
                               Margin="0,20,0,15"/>

                    <Border Background="#FF2C3E50" 
                            CornerRadius="5" 
                            Padding="10" 
                            MaxHeight="300">
                        <ScrollViewer VerticalScrollBarVisibility="Auto">
                            <TextBlock Text="{Binding RelatorioGeral}" 
                                       Foreground="#FFBDC3C7" 
                                       FontSize="10" 
                                       FontFamily="Consolas" 
                                       TextWrapping="Wrap"/>
                        </ScrollViewer>
                    </Border>

                </StackPanel>
            </ScrollViewer>
        </Border>

        <!-- Barra de Status -->
        <Border Grid.Column="0" Grid.ColumnSpan="3" Grid.Row="2" 
                Style="{StaticResource StatusBar}">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <TextBlock Grid.Column="0" 
                           Text="{Binding StatusCompleto}" 
                           Foreground="#FFBDC3C7" 
                           FontSize="10" 
                           VerticalAlignment="Center"/>

                <TextBlock Grid.Column="1" 
                           Text="{Binding VersaoSistema}" 
                           Foreground="#FF7F8C8D" 
                           FontSize="10" 
                           VerticalAlignment="Center"/>
            </Grid>
        </Border>

    </Grid>
</Window>