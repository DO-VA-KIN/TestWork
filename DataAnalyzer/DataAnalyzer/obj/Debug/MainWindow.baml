   M S B A M L   `   `   `  ????G  CDataAnalyzer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:DataAnalyzerDataAnalyzer  5   	   M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089M3clr-namespace:System.ComponentModel;assembly=SystemSystem.ComponentModel 5   	   E'clr-namespace:DataAnalyzer.UserControlsDataAnalyzer.UserControls  5   	       DataAnalyzer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:DataAnalyzer     5   	   Bcm3clr-namespace:System.ComponentModel;assembly=System     5   	   @UserControls'clr-namespace:DataAnalyzer.UserControls     5   	     ? Title$/  )Анализатор данных (v.1.1)??5
   	   $	??500??6+   $	??800??68    ? Icon$ /Icons/IMG_0200.PNG??6D   ??5      ?? 5   
    ?? EndPoint$	 0,1&?6    ?? 
StartPoint$	 0,0&?6-    ?? Opacity$	 0.2Q?6>   .??5      ?  ? Color$ 
WhiteSmoke??6    ? Offset$ 0Q?6.   ? 5      $ 	LightGray??6   $ 1Q?6-   5      .??5      ?  J? RenderTransformOrigin$ 0.5,0.5&?6   z?5   
   V? 5      .~?5      ?? ?? 5      ?? 5      T? 5       T? X$ -2Q?6%   5      	 ? RowDefinitions	 5   
   ?? 5      $??22 ?6   ?? 5      -   5        
 ??Name$
 GridRow1??5      $??7* ?6.   $??50??6:   ?? 5      $	??78* ?6   $	??150??6)   ?? 5      -   5        $
 GridRow3??5      $	??15* ?6.   $??75??6;    ? ColumnDefinitions 5!   
   ?? 5"      $??1* ?6   ?? 5#      $??4* ?6   ?? 5$      $??1* ?6   .+?5'   
   o? $??12(?6   $??Stretch=?6   $??Stretch=?6;   $??0??6W   $??0??6d   $??3??6t   ??5(      ?? 5)      $ 0.5,1&?6&   $ 0.5,0&?67   .??5*      ? $ Black??6#   $ 0Q?61   ? 5+      $
 Gray??6#   $ 1Q?60   5,      .@?5.      m? -   5         m?Name$ 	Menu_File??5.      $??Файл??6*   .@?5/      m? -   5        $??Расположение??5/      
 ? Tag$ 0??61   69   m? 50      -   5        $??Справка??50      $ 1??6,   64   51      m? 52      -   5        $ 	Menu_View??52      $??Вид??6*   .@?53      m? -   5        $??
Файлы??53       m? 	IsChecked . $ 0??6;   6C   m? 54      -   5        $??Таблица??54       . $ 1??6=   6E   m? 55      -	   5        $??Статус??55       . $ 2??6<   6D   m? 56      -
   5        $??
Сброс??56       .  $ 3??6<   6D   57      58      ?? 5;   
   -   5         ??Name$ ChConfigFileXML??5;      $
??Left=?6,   $??Center=?6G   {?.  $??1??6t   $??1??6?   $ 0.5,0.5&?6?   z?5<      V? 5=      .~?5>      ??  ?? ScaleX$	 1.2Q?6%    ?? ScaleY$	 1.2Q?62   5?      5A      ?? 5B   
   -   5         ??Name$ BtnConfigFileXML??5B      $??Конфигурация??6+    ? ToolTip$G AВыбор файла xml c кадровой структурой??6B   $??Stretch=?6r   $??Stretch=?6?   $??1??6?   $??0??6?   $??5,2,5,2q?6?   6?   ?? 5C   
   -   5        $ BtnDataFile??5C      $??Данные??6&   $, &Выбор файла c данными??67   $??Stretch=?6W   $??Stretch=?6u   $??1??6?   $??2??6?   $??5,2,5,2q?6?   6?   &   System.Windows.Controls.DataGrid  5E   
   -   5          Name$ DGFrames??5E         AlternatingRowBackground
 ?????$??Times New Roman)?6J   $??14(?6g      HeadersVisibility$
 None=?6u   $   HorizontalScrollBarVisibility$ Visible=?6?   $??Stretch=?6?   $??Stretch=?6?      SelectionMode$ Single=?6?      AutoGenerateColumns . $??2??6:  $??0??6G  $??3??6W  ??6"  ?? 6W  ?? 5G   
   $??Stretch=?6   $??Stretch=?61   $??3??6M   $??0??6Z   $??3??6j   $??0,0,0,0q?6~   .??5H         DataAnalyzer.UIProcess  -   5          Name$ 	UIPStatus??5H      5I      5J      5K      