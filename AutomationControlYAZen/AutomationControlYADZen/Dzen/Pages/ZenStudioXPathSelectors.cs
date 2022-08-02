namespace AutomationControlYADZen.Dzen.Pages;

public static class ZenStudioXPathSelectors
{
    public const string Add =
        "//button[@class='author-studio-header__addButton-1Z author-studio-header__rightItemButton-3a']";

    public const string CreatePost =
        "//span[@class='new-publication-dropdown__buttonTitle-3l new-publication-dropdown__themePortal-1m'][contains(text(), 'Написать статью')]";

    public const string PopUp = "//span[@class='help__link help__link_placeholder'][contains(text(), 'Текст')]";
    public const string Zagolovok = "//h1//div[@class='public-DraftStyleDefault-block public-DraftStyleDefault-ltr']";
    public const string TextBody = "//div[@class='zen-editor-block zen-editor-block-paragraph']";

    public const string Opublikovat =
        "//button[@class='Button2 Button2_view_action Button2_size_s editor-header__edit-btn']";

    public const string PicSelect = "//div[@class='cover-picker__cover'][1]";

    public const string OpublikovatOk =
        "//span[@class='ui-lib-button__content-wrapper'][contains(text(), 'Опубликовать')]";

    public const string ChannelNameInputField = "//input[@class='Textinput-Control']";
    public const string ChannelNameSave = "//button[@type='submit']";

    public const string SetingButton1 =
        "//span[@class='Text Text_typography_text-14-18 navbar__text-3G'][contains(text(), 'Настройки')]";

    public const string SetingButton2 = "//button/*[contains(text(), 'Настройки')]";
    public const string GoToChannelButton = "//span[@class='Link Link_view_default Link_theme_button']";
    public const string SettingsWindowOpened = "//h2[contains(text(), 'Настройка канала')]";

    public const string CloseSettingsWindowButton =
        "//button[@class='Button2 Button2_view_promo Button2_size_s Button2_pin_circle-circle Button2_icon_only Modal-CloseButton']";
}

