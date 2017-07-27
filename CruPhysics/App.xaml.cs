using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CruPhysics.PhysicalObjects;

namespace CruPhysics
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            PhysicalObjectManager.ScanPhysicalObjectType(Assembly.GetAssembly(typeof(PhysicalObjectManager)));
        }


        /// <summary>
        /// Find the <see cref="TextBox"/> being validated from <paramref name="errorTextBlock"/>.
        /// </summary>
        private static TextBox FindValueTextBox(TextBlock errorTextBlock)
        {
            var parent = VisualTreeHelper.GetParent(errorTextBlock) as FrameworkElement;
            return (parent.FindName("Placeholder") as AdornedElementPlaceholder).AdornedElement as TextBox;
        }

        private void ErrorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var errorTextBlock = sender as TextBlock;


            Action beginAnimation = () =>
            {
                errorTextBlock.Visibility = Visibility.Visible;

                var storyboard = (FindResource("ErrorTextBlockStoryboard") as Storyboard).Clone();
                var opacityAnimation = storyboard.Children[0] as DoubleAnimation;
                Storyboard.SetTarget(opacityAnimation, errorTextBlock);
                opacityAnimation.Completed += (sender1, e1) =>
                {
                    errorTextBlock.Visibility = Visibility.Collapsed;
                    storyboard.Remove();
                };
                storyboard.Begin();
            };

            EventHandler<ValidationErrorEventArgs> handler = (s, args) =>
            {
                beginAnimation();
            };

            //Use Tag to store the handler!
            errorTextBlock.Tag = handler;

            beginAnimation();

            Validation.AddErrorHandler(FindValueTextBox(errorTextBlock), handler);
        }

        private void ErrorTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            var errorTextBlock = sender as TextBlock;
            Validation.RemoveErrorHandler(FindValueTextBox(errorTextBlock), (EventHandler<ValidationErrorEventArgs>)errorTextBlock.Tag);
        }
    }
}
