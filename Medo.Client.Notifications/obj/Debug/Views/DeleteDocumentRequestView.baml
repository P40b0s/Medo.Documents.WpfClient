   M S B A M L   `   `   `  ����  Medo.Client.Notifications?    9Medo.Client.Notifications.Views.DeleteDocumentRequestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $G Apack://application:,,,/Medo.Core;component/Resources/Buttons.xaml?�6%   %    5      .��5      � $	��300��6   $	��530��6   
������ .+�5      � $	��270��6   $	��500��6   
������� K� ZIndex$ 1��67    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��100* �6   �� 5      $��40 �6   .+�5   
   �� $��0��6   $	��Top=�6!   $��16(�69   ���	#FFD9FFA2$
��Bold"�6^   $��Arial Black)�6p   �� 5      $!�Подтверждение ��6    �� 5      �6   �� *Notification.Title+5      �� 5   
    �� Orientation$ Vertical=�6   $��1��6,   $��	0,10,0,10q�69   .+�5      �� $
��Wrap=�6   $��16(�6,   �� 5      $%�Принявший орган: ��6   $
��Bold"�6/    �� 5      �6   �� **(Notification.OperationDocument.OrganName+5      �� 5       $
��Wrap=�6   $��16(�6,   �� 5!      $!�Вид документа: ��6   $
��Bold"�6-    �� 5"      �6   �� *(&Notification.OperationDocument.ActType+5#      �� 5%      $
��Wrap=�6   $��16(�6,   �� 5&      $%�Номер документа: ��6   $
��Bold"�6/    �� 5'      �6   �� */-Notification.OperationDocument.DocumentNumber+5(      	� 5*      $C��=Зарегистрировать документ в СЭДе��6   $��16(�6J    	� 	GroupName$	 reg��6X   $��0 5 0 0q�6�    d� 	IsChecked 6h   �� *$"Notification.RegisterDocumentInSED+	 �� Mode$	 TwoWay=�6�   	� 5+      $3��-Удалить документ из СЭДа��6   $��16(�6C   $	 reg��6Q    6a   �� *$"Notification.DeleteDocumentFromSED+$	 TwoWay=�	� 5,      $C��=Отказать документу в регистрации��6   $��16(�6K   $	 reg��6Y    6i   �� *&$Notification.RejectRegistrationInSED+$	 TwoWay=��� 5-      $��16(�6   ��5.      �� *Notification.RejectStatuses+
 �� SelectedItem
 5/      �� *Notification.RejectStatus+��50      �� 51       �� 
TargetType" ��6    .�52      ��  �� Property � y� �� Value$ 	Collapsed=�6;    �� Triggers 53      �� 54   "    �� Binding 6.   �� *&$Notification.RejectRegistrationInSED+ �� Value true6g   .a�55   &   ��  � y�$ Visible=�6C   56   #   58      5:      5;      �� 5>   
   ���	#FFD9FFA2$��10 0 0 5q�5@         OkButton#��[  5A      $	��100��5B      $��18(�5C      $��2��5D      $
��Left=�5E      ��5F      �� *	OkCommand+.��5G      Принять6   �� 5H      $��0 0 10 5q�5I        CancelButton#��[ 5J      $	��100��5K      $��18(�5L      $��2��5M      $��Right=�5N      ���	#FFD9FFA2��5O      �� *CancelCommand+.��5Q      Отмена6   5R      �� 5S   
   ���	#FFD9FFA2$��20q�6)    J� Opacity$	 0.7Q�6>   $ 0��6L    J� Effect 5T      -  'System.Windows.Media.Effects.BlurEffect  5U         Radius$ 10Q�6   5W      5X      5Y      