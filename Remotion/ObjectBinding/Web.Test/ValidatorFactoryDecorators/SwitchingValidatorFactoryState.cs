namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingValidatorFactoryState
  {
    private static SwitchingValidatorFactoryState _instance;
    public static SwitchingValidatorFactoryState Instance
    {
      get { return _instance ?? (_instance = new SwitchingValidatorFactoryState()); }
    }

    public bool UseFilteringFactory { get; set; }
  }
}