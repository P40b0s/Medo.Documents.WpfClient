   M S B A M L   `   `   `  ���� I  ETrayInfoModule, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:Medo.Modules.TrayInfoModule.Converters&Medo.Modules.TrayInfoModule.Converters  ;    5Medo.Modules.TrayInfoModule.Views.NotificationControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    I
convertres4clr-namespace:Medo.Modules.TrayInfoModule.Converters    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � 	Resources  A  ;Medo.Modules.TrayInfoModule.Converters.ByteToImageConverter   btoimgconverter  NotificationStyle �� 
TargetType �� Property �� Value �� RelativeSource �� AncestorType �� 
TargetType ��Name �� Orientation	 J� Opacity
 �� ElementName �� Path �� 	Converter �� TextAlignment �� StringFormat �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value :� RoutedEvent �� TargetProperty g� Duration W� From	 W� To��%�  &	        &	      7     �� " ��.���  � ��
 �   ���  � �� �� *HorizontalContentAlignment+ �� " ����  � �� �� *VerticalContentAlignment+ �� " ����  � �� �� " ��.R��� $ 
mainBorder��
������ $��2q�._��� $ 
Horizontal=�$	��450��
�������$
	 0.85Q�.+��� $��0,0,1,0q����� $
 
mainBorder��$ BorderBrush�._��� $��60������ *
SourceGuid+8   �� $��0,0,1,0q����� $
 
mainBorder��$ BorderBrush�._��� $
��Wrap=�$	��150��$ Center=�$��Center=�$��Calibri)�$
��Bold"�$��14(����� *	OrganName+�� $��0,0,1,0q����� $
 
mainBorder��$ BorderBrush�._��� $
��Wrap=�$	��100��$ Center=�$��Center=�$��Calibri)�$
��Bold"�$��14(����� *	ActType+�� $��0,0,1,0q����� $
 
mainBorder��$ BorderBrush�._��� $
��Wrap=�$��70��$ Center=�$��Center=�$��Calibri)�$
��Bold"�$��14(����� *	ActNumber+�� $
��Wrap=�$��70��$ Center=�$��Center=�$��Calibri)�$
��Bold"�$��14(����� *DeliveryTime+$ {0:dd.MM.yyyy HH:mm:ss}�� P�  �@��
IsSelected . . ��� $ 
mainBorder��# � �� � ����  � ��
 �   �P�  �@J�IsMouseOver . . ��� $ 
mainBorder�� � ��
 � � �]�  ��  �@��
IsSelected . ��  �@��IsSelectionActive .  ./��� $ 
mainBorder��# � �� � ����  � ��
 �   �P�  � {� .  . ��� # � �� � ��:� $ FrameworkElement.Loaded��.[��� .���� .��W� $ Opacity�$ 00:00:02D�$ 0X�$ 1X�:� $ FrameworkElement.Unloaded��.[��� .���� .��W� $ Opacity�$ 00:00:02D�$ 1X�$ 0X�.���� -    ��Name$ itemsListBox�� �� SelectionMode$ Single=�
������ 
������ #��[ ���� *NotificationsCollection+