namespace OmniApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

    // Das ist der neue Weg für .NET 10, um die Startseite festzulegen
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage());
	}
}