   M S B A M L   `   `   `  ����J  FPdfViewerModule, Version=1.4.0.1, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Medo.Modules.PdfViewerModuleMedo.Modules.PdfViewerModule  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullc _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� Source   ItemsControlStyle �� 
TargetType �� BasedOn �� Property �� Value �� Triggers �� Binding �� Path	 �� Mode
 �� Value N� EnterActions �� ResourceKey N� ExitActions  RectangleDataTemplate �Name �� ElementName � RowDefinitions � Triggers :� RoutedEvent g� 	BeginTime �� 
TargetName �� TargetProperty R� KeyTime   System.Windows.Visibility    Visible R� Value	 �� To g� Duration    Hidden K� ZIndex �� RelativeSource �� AncestorType  MedoClientCancelRedIcon ��Name  ItemsControlRectangleStyle  ItemsControlContainerStyle�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      ;Controls,http://metro.mahapps.com/winfx/xaml/controls  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      5static*clr-namespace:Medo.Modules.PdfViewerModule  5      $prismhttp://prismlibrary.com/  5        5	      �� 5
   
   $P Jpack://application:,,,/PdfViewerModule;component/Templates/Animations.xaml?�6   �� 5   
   $` Zpack://application:,,,/PdfViewerModule;component/Templates/ItemsControlRectangleStyle.xaml?�6   �� 5   
   $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?�6   �� 5   
   $F @pack://application:,,,/Medo.Core;component/Resources/Button.xaml?�6   %8	  &	        7��0�� 5      $ StartProgressAnimation��6-   10�� 5!      $ StopProgressAnimation��6-   1&	 �    7  7  &	     �� 5      " ��6'   8   .�5   
   ��  �@J�Opacity$ 0Q�6$   �� 5   
    � y�$ 	Collapsed=�6'   �� 5   
    �@K�ZIndex$ 3��6)    5   
   �� 5       6   �� $- '(static:StaticProperty.RecognitionMode)�$	 OneWay=�
 true6h   .a�5      ��  � y�$ Visible=�6/    5      �� 5      .��5      �� .��5      2 5      5       5      �� 5      .��5       �� .��5!      2 5"      5#      5%      5'      �� 5*      .R�5+   
   � $ mainGrid��6   
������ ��6:   �� $
 rect��$ ActualWidth� 5,      �� 5-      $
��auto �6    �� 5.      $
��auto �6     50      :� 51      $ 	MouseMove��6   .[�52      �� .��53      �� .��54      T� $ 00:00:00X�6<   $ 
selectGrid��6Q   $ 
Visibility�6t   ..�55   "   i� $ 00:00:00��69   # Z 6L   56      �� 57      $
 rect��6-   $. ((Rectangle.Fill).(SolidColorBrush.Color)�6J   $ 	#4CFF2300X�6�   $ 0:0:0.3D�6�   58      59      5:      :� 5;      $ 
MouseLeave��6   .[�5<      �� .��5=      �� .��5>      T� $ 00:00:00X�6<   $ 
selectGrid��6Q   $ 
Visibility�6t   ..�5?   "   i� $ 00:00:00��69   # Z 6L   5@      �� 5A      $
 rect��6-   $. ((Rectangle.Fill).(SolidColorBrush.Color)�6J   $ 	#4CB5F16CX�6�   $ 0:0:0.3D�6�   5B      5C      5D      .+�5G      � $ 
selectGrid��6,   $��20��6   $��0��6   ���	#B2FFFFFF$y�Hidden=�6X   ��6l   �� $
 rect��$ ActualWidth�.+�5H      �� $#��Распознать блок��6   $��18(�5I      
���   �$ 0��5K      ��5L      ��  �� " ��$1 +DataContext.GetBitmapForCurrentBlockCommand���5M      �� $ mainGrid��$ DataContext��� 5W      $ 1��6   $��Right=�5X      ��5Y      ��  �� " ��$+ %DataContext.DeleteCurrentBlockCommand���5Z      �� $ mainGrid��$ DataContext�.��5[      �� 8��  6   5\      5]      �� 5^      $
 rect��6   $��1��6'   8�� 65   5t      5u      �� 5w      " ��60   .�5x   
   ��  �@��Left 6(   �� *ControlXCoords+�� 5y   
    �@��Top 6'   �� *ControlYCoords+5z      5|      