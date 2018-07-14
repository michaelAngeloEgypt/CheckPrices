using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

public class ItemElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
{
    protected override ConfigurationElement CreateNewElement()
    {
        return new T();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
        return ((T)element);
    }
}

public partial class SettingsConfigurationSection : ConfigurationSection
{
    public static SettingsConfigurationSection Settings(string Section)
    {
        return (SettingsConfigurationSection)ConfigurationManager.GetSection(Section);
    }

    public static string AppSetting(string Setting)
    {
        if (ConfigurationManager.AppSettings.AllKeys.Contains(Setting))
            return ConfigurationManager.AppSettings[Setting];

        return "";
    }
}

public class Sites : ConfigurationElement
{
    [ConfigurationProperty("Site", IsKey = true, IsRequired = true)]
    public string Site
    {
        get { return (string)this["Site"]; }
    }

    [ConfigurationProperty("URL", IsKey = false, IsRequired = true)]
    public string URL
    {
        get { return (string)this["URL"]; }
    }

    [ConfigurationProperty("Match", IsKey = false, IsRequired = true)]
    public string Match
    {
        get { return (string)this["Match"]; }
    }

    [ConfigurationProperty("Group", IsKey = false, IsRequired = true)]
    public string Group
    {
        get { return (string)this["Group"]; }
    }
}

public class Articles : ConfigurationElement
{
    [ConfigurationProperty("Site", IsKey = true, IsRequired = true)]
    public string Site
    {
        get { return (string)this["Site"]; }
    }

    [ConfigurationProperty("HGMLiftParts_Code", IsKey = true, IsRequired = true)]
    public string HGMLiftParts_Code
    {
        get { return (string)this["HGMLiftParts_Code"]; }
    }

    [ConfigurationProperty("Code", IsKey = false, IsRequired = true)]
    public string Code
    {
        get { return (string)this["Code"]; }
    }

    public double? HGMLiftParts_Price { get; set; }

    public double? Price { get; set; }
}

public partial class SettingsConfigurationSection : ConfigurationSection
{
    [ConfigurationProperty("Sites")]
    private ItemElementCollection<Sites> _Sites
    {
        get { return (ItemElementCollection<Sites>)this["Sites"]; }
    }

    public List<Sites> Sites
    {
        get { return _Sites.ToList(); }
    }

    [ConfigurationProperty("Articles")]
    private ItemElementCollection<Articles> _Articles
    {
        get { return (ItemElementCollection<Articles>)this["Articles"]; }
    }

    public List<Articles> Articles
    {
        get { return _Articles.ToList(); }
    }
}

public static class ItemElementCollectionExtensions
{
    static public List<Sites> ToList(this ItemElementCollection<Sites> itemElementCollection)
    {
        return (from y in itemElementCollection.OfType<Sites>() select y).ToList();
    }

    static public List<Articles> ToList(this ItemElementCollection<Articles> itemElementCollection)
    {
        return (from y in itemElementCollection.OfType<Articles>() select y).ToList();
    }
}