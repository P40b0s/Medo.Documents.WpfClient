   M S B A M L   `   `   `  ����  Medo.Client.Notifications;    5Medo.Client.Notifications.Views.CardEditorRequestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $W Qpack://application:,,,/Medo.Client.Notifications;component/Templates/Buttons.xaml?�6%   �� 5      $U Opack://application:,,,/Medo.ImageResources;component/PNGResourceDictionary.xaml?�6%   �� 5      $K Epack://application:,,,/Medo.ImageResources;component/Backgrounds.xaml?�6%   %    5      .��5      � $
��auto��6   $
��auto��6    K� ZIndex$ 1��6&   $	��500��67   ��5   
   �� 5         Green_Light �� ImageSource# [  6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $
��auto �6   .+�5   
   � 
������ $��0��5      .+�5      �� $
��Left=�6     UpdateCenterIcon#��[ 6/   �� 5       $��Center=�6=   $
��Left=�6X   $��30, 0, 0, 0q�6s   $��16(�6�   $
��Bold"�6�   $��Arial Black)�6�   ��6   �� *Notification.Title+6�   �� 5!      $��0 0 0 2q�5"        CancelButton#��[ 5#      $��18(�5$      $��0��5%      $��Right=�5&      ��5'      �� *CancelCommand+.��5(      ��   MedoClientCancelRedIcon#��[ 6   5)      5+      �� 5-   
   !  Prism.Regions.RegionManager   
RegionName$ CardEditorRegion��6   $��1��6L   5;      5<      