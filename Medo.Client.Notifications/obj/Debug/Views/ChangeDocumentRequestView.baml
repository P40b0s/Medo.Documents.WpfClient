   M S B A M L   `   `   `  ����  Medo.Client.Notifications?    9Medo.Client.Notifications.Views.ChangeDocumentRequestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source   
itemsStyle �� 
TargetType �� BasedOn �� Property �� Value �� Mode	 �� Triggers
 P� Property P� Value  flashListItem � RowDefinitions 	� 	GroupName d� 	IsChecked � Maximum � Value�� 5   
    5	      �� 5
      $G Apack://application:,,,/Medo.Core;component/Resources/Buttons.xaml?�6%   %�  &	        7��&	     �� 5      " ��6'   8   .�5      ��  �@��
IsSelected 6/   �� *
IsSelected+$ TwoWay=�	 5      P� 5      
 �@��
IsSelected . . �5      ��  � �� �	#FFD9FFA267   �� 5       � �� �	#FFFF840867   5      5      �� 5      .R�5      �  5      �� 5      $��25 �6(   �� 5      $��5 �6(   .+�5      	� $ flash��6"   $��18(�5   "   $��Arial)�5   "   
������ 
���   �$��0��5"   "   ��5   "   �� *Name+ 5   "   �� *
IsSelected+5"   "   � 5#      $��1��5%   "    5#   "   �� *
TotalSpace+ 5$   "   �� *
TakenSpace+5%   "   5&      5(      5)      .��5,      � $
��auto��6   $
��auto��6   
������ .+�5-      � $
��auto��6   $
��auto��6   
������� K� ZIndex$ 1��69   $��15q�6J   $	��500��6V    5.   
   �� 5/      $��20 �6   �� 50      $
��auto �6   �� 51      $
��auto �6   �� 52      $��40 �6   .+�54   
   �� $��0��6   $	��Top=�6!   $��16(�69   ���	#FFD9FFA2$
��Bold"�6^   $��Arial Black)�6p   �� 55      $!�Подтверждение ��6    �� 56      �6   �� *Notification.Title+57      �� 58   
    �� Orientation$ Vertical=�6   $��1��6,   $��	0,10,0,10q�69   .+�59      �� $
��Wrap=�6   $��16(�6,   �� 5:      $%�Принявший орган: ��6   $
��Bold"�6/    �� 5;      �6   �� **(Notification.OperationDocument.OrganName+5<      �� 5>      $
��Wrap=�6   $��16(�6,   �� 5?      $!�Вид документа: ��6   $
��Bold"�6-    �� 5@      �6   �� *(&Notification.OperationDocument.ActType+5A      �� 5C      $
��Wrap=�6   $��16(�6,   �� 5D      $%�Номер документа: ��6   $
��Bold"�6/    �� 5E      �6   �� */-Notification.OperationDocument.DocumentNumber+5F      	� 5H      $C��=Зарегистрировать документ в СЭДе��6   $��16(�6J   $	 reg��6X   $��0 5 0 0q�6�    6h   �� *$"Notification.RegisterDocumentInSED+$ TwoWay=�6�   	� 5I      $3��-Удалить документ из СЭДа��6   $��16(�6C   $	 reg��6Q    6a   �� *$"Notification.DeleteDocumentFromSED+$ TwoWay=�	� 5J      $C��=Отказать документу в регистрации��6   $��16(�6K   $	 reg��6Y    6i   �� *&$Notification.RejectRegistrationInSED+$ TwoWay=��� 5K      $��16(�6   ��5L      �� *Notification.RejectStatuses+ �� SelectedItem 5M      �� *Notification.RejectStatus+��5N      �� 5O      " ��6    .�5P      ��  � y�$ 	Collapsed=�6;   	 5Q      �� 5R   "    �� Binding 6.   �� *&$Notification.RejectRegistrationInSED+ �� Value true6g   .a�5S   &   ��  � y�$ Visible=�6C   5T   #   5V      5X      5Y      �� 5Z      $3��-Доступные флэш носители:��6   $��2��68   $��16(�6F   �� 5[      #��[  5]      #��[ 5^      $��2��5_      $��0 22 0 0q�5`      ��5[      �� *!Notification.MountingFlashDisks+ 5\      �� * Notification.SelectedFlashDisk+5a      �� 5c      ���	#FFD9FFA2$��10 0 0 5q�5e        OkButton#��[ 5f      $	��100��5g      $��18(�5h      $��3��5i      $
��Left=�5j      ��5k      �� *	OkCommand+.��5l      Принять6   �� 5m      $��0 0 10 5q�5n        CancelButton#��[ 5o      $	��100��5p      $��18(�5q      $��3��5r      $��Right=�5s      ���	#FFD9FFA2��5t      �� *CancelCommand+.��5v      Отмена6   5w      �� 5x   
   ���	#FFD9FFA2$��20q�6)    J� Opacity$	 0.7Q�6>   $ 0��6L    J� Effect 5y      -  'System.Windows.Media.Effects.BlurEffect  5z         Radius$ 10Q�6   5|      5}      5~      