   M S B A M L   `   `   `  ����R  NDocumentsUploaderModule, Version=1.4.0.0, Culture=neutral, PublicKeyToken=nullq=clr-namespace:Medo.Modules.DocumentsUploaderModule.Converters/Medo.Modules.DocumentsUploaderModule.Converters  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullc _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� SourceJ   DMedo.Modules.DocumentsUploaderModule.Converters.ByteToImageConverter   BytesToImageConverter  ListBoxItemBackground �� 
TargetType �� Property �� Value �� Triggers �� Binding �� Value  ProgressBarsBackgroundColor :� RoutedEvent	 �� TargetProperty
 g� Duration W� From	 W� To  ListItem �� DataType �Name � ColumnDefinitions � RowDefinitions �� StretchDirection �� 	Converter �� Orientation ��Name �� Triggers �� 
TargetName�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      ;Controls,http://metro.mahapps.com/winfx/xaml/controls  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      L
converters=clr-namespace:Medo.Modules.DocumentsUploaderModule.Converters  5      8metro,http://metro.mahapps.com/winfx/xaml/controls  5      $prismhttp://prismlibrary.com/  5        5
      �� 5   
   $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?�6   �� 5   
   $T Npack://application:,,,/DocumentsUploaderModule;component/Templates/Colors.xaml?�6   %�	  &	        &	      7  &	 t    7      5      6&   �� 5      " ��6*   .�5   
   ��  � �� �	#FFE8F7B36'    5   
   �� 5       6   �� *IsCopyComplete+	 false6=   .a�5      ��  � ��8   6/   5      :� 5      $ FrameworkElement.Loaded��6   .[�5      �� .��5      �� .��5      W� $	 Opacity�6*   $
 00:00:03D�5   *   $ 0X�5   *   $ 1X�5   *   5      5       5!      :� 5#      $ FrameworkElement.Unloaded��6   .[�5$      �� .��5%      �� .��5&      W� $	 Opacity�6*   $
 00:00:02D�5'   *   $ 1X�5(   *   $ 0X�5)   *   5*      5+      5,      5.      �� 50      " ��6$   .R�51   
   �� ���	#CCFFFFFF$��0,0,0,3q�6)   ._�52      � $
 grid��6    53      �� 54      $��90 �6'   �� 55      $
��auto �6'    57      �� 58      $��15 �6$   �� 59      $
��auto �6$   .+�5B      �� $��Center=�5C      $��89��5D      $��89��5E      $��Center=�5F      $��0��5G      $��1��5H      $��UniformToFill=�5I      $
 Both=�5J      ��5B      �� *
SourceGuid+8   5K      �� 5N      5P      �� 5Q      $ Vertical=�6   $��1��64   $��1��6D   .+�5R      �� $
��Wrap=�6"   
���   �$��Calibri)�6I   $��16(�6^   �� 5S      $%�Принявший орган: ��6   $
��Bold"�68    �� 5T      �6   �� *	OrganName+5U      �� 5V      $
��Wrap=�6!   
���   �$��Calibri)�6H   $��16(�6]   �� 5W      $!�Вид документа: ��6   $
��Bold"�66    �� 5X      �6   �� *	ActType+5Y      �� 5Z      $ 	pakNumber��6    $
��Wrap=�66   
���   �$��Calibri)�6]   $��16(�6r   �� 5[      $�Номер МЭДО: ��6   $
��Bold"�63    �� 5\      �6   �� *DocumentNumber+5]      �� 5^      $ 	izdNumber��6    $
��Wrap=�63   
���   �$��Calibri)�6Z   $��16(�6o   �� 5_      $%�Номер документа: ��6   $
��Bold"�68    �� 5`      �6   �� *ChangedNumber+5a      �� 5b      
���   �$��Calibri)�6U   $��16(�6j   
������ ��6   �� *CopyMessage+6x   5c      5e      5f       5g   
   �� 5h       6   �� *DocumentNumber+ 6=   Z� .a�5i      �� $ 	pakNumber��6/    � y�$ 	Collapsed=�6G   5j      �� 5k       6   �� *ChangedNumber+ 6<   Z� .a�5l      �� $ 	izdNumber��6/    � y�$ 	Collapsed=�6G   5m      5�      5�      