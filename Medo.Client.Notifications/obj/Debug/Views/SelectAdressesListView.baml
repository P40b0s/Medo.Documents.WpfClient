   M S B A M L   `   `   `  ����  Medo.Client.Notifications<    6Medo.Client.Notifications.Views.SelectAdressesListView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $f `pack://application:,,,/Medo.Client.Notifications;component/Templates/ReportsSenderListStyle.xaml?�6%   %    5      .��5      � $
��auto��6   $
��auto��6   
������� K� ZIndex$ 1��69   $	��800��6J    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��auto �6   �� 5      $��40 �6   .+�5   
   �� $��0��6   $	��Top=�5      $��16(�5      ���	#FFD9FFA2$
��Bold"�5      $��Arial Black)�5      ��5      �� *Notification.Title+5      �� 5   
   $	��865��5         
itemsStyle#��[  5!      $��1��5"      ��5       �� *Notification.AdressList+ �� NotifyOnSourceUpdated .  �� Mode$ TwoWay=� �� IsAsync . 	 �� View	 5#      �� 5$      .P�5%      �� $��
Адрес��6%     CheckTemplate
 �� CellTemplate#
 [ 64    �� Width$	 300��6c   �� 5&      $��
Орган��6%     OrganTemplate#
 [ 64   $	 700��6c   5'      5)      �� 5+   
   ���	#FFD9FFA2$��18(�5-      $��2��5.      ��5/      �� *	OkCommand+.��50      Принять6   5:      5;      