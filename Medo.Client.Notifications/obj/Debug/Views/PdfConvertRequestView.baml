   M S B A M L   `   `   `  ����  Medo.Client.Notifications;    5Medo.Client.Notifications.Views.PdfConvertRequestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $G Apack://application:,,,/Medo.Core;component/Resources/Buttons.xaml?�6%   %    5      .��5      � $
��auto��6   $
��auto��6   
������ .+�5   
   � $
��auto��6   $
��auto��6   
������� K� ZIndex$ 1��6=   $��15q�6N   $	��500��6Z    � RowDefinitions 5      �� 5      $��20 �6    �� 5      $
��auto �6    �� 5      $
��auto �6    �� 5      $��40 �6    .+�5      �� $��0��6   $	��Top=�6%   $��16(�6=   ���	#FFD9FFA2$
��Bold"�6b   $��Arial Black)�6t   �� 5      $(�"Конвертация файла ��6    �� 5      �6   �� *!Notification.PdfForConvert.Name+ �� Mode$ OneWay=�5      �� 5       �� Orientation$ Vertical=�6   $��1��60   $��	0,10,0,10q�6=   .+�5      �� $��Цветной PDF��6S   $��16(�6j    d� 	IsChecked 6   �� *Notification.IsColor+$ TwoWay=�6j   � 5      $��1��6   .+�5      � $��50��5       	 � Maximum	 5   "   �� *Notification.PagesMaximum+
 � Value
 5   "   �� *" Notification.ConvertedPagesCount+5       �� 5"      $
��Wrap=�6     �� TextAlignment$ Center=�64   $��Center=�6K   $��16(�6f   ��6t   �� *Notification.Status+5#      �� 5$      $4��.Начать конвертацию файла��6   $��16(�6<   ��6J   �� *StartConvertCommand+5&      �� 5*      ���	#FFD9FFA2$��10 0 0 5q�5,         OkButton#��[  5-      $	��100��5.      $��18(�5/      $��3��50      $
��Left=�51      ��52      �� *	OkCommand+.��53      Принять6   �� 54      $��0 0 10 5q�55        CancelButton#��[ 56      $	��100��57      $��18(�58      $��3��59      $��Right=�5:      ���	#FFD9FFA2��5;      �� *CancelCommand+.��5=      Отмена6   5>      �� 5?   
   ���	#FFD9FFA2$��20q�6)    J� Opacity$	 0.7Q�6>   $ 0��6L    J� Effect 5@      -  'System.Windows.Media.Effects.BlurEffect  5A         Radius$ 10Q�6   5C      5D      5E      