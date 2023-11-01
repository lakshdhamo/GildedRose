using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    private readonly IList<Item> _items;
    private readonly int? _maxQuality;
    private ItemCategory _itemCategory = ItemCategory.NormalItem;
    private int _qualityFactor = Constants.DefaultQualityDegradeFactor;
    private delegate int CalculateQuality(Item item);

    /// <summary>
    /// Initialize the default values
    /// </summary>
    /// <param name="items">Items to be processed</param>
    /// <param name="maxQuality">Optional value. Denotes maximum quality value</param>
    public GildedRose(IList<Item> items, int? maxQuality = null)
    {
        _items = items;
        _maxQuality = maxQuality ?? Constants.DefaultMaxQuality;            // Maximum value is not provided then default value is considered
    }

    /// <summary>
    /// Update quality based on Items
    /// </summary>
    public void UpdateQuality()
    {
        foreach (var item in _items)
        {
            ValidateItem(item);                                             // Validate the items
            Prerequisite(item);                                             // Perform prerequisite operation
            UpdateItem(item);                                               // Update items
        }
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="item"></param>
    private void UpdateItem(Item item)
    {
        item.Quality = GetQuality(item);
        item.SellIn = GetSellIn(item);
    }

    /// <summary>
    /// Logic to calculate SellIn value
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int GetSellIn(Item item)
    {
        return _itemCategory == ItemCategory.LegendaryItem ? item.SellIn : --item.SellIn;
    }

    /// <summary>
    /// Logic to calculate Quality based on items' category
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int GetQuality(Item item)
    {
        int result;
        CalculateQuality calculateQuality = _itemCategory switch
        {
            ItemCategory.AgedItem => AgedItemQuality,
            ItemCategory.LegendaryItem => LegendaryItemQuality,
            ItemCategory.ConcertItem => ConcertItemQuality,
            ItemCategory.ConjuredItem => ConjuredItemQuality,
            _ => NormalItemQuality,
        };
        result = calculateQuality(item);
        return NormalizeQuality(result);
    }

    /// <summary>
    /// Normalize quality value. Quality never be negative and more than MaxQuality except LegendaryItem
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private int NormalizeQuality(int result)
    {
        if (_itemCategory != ItemCategory.LegendaryItem)
        {
            if (result < 0)
                result = 0;
            if (result > _maxQuality)
                result = (int)_maxQuality;
        }
        return result;
    }

    #region Calculate Quality
    /// <summary>
    /// Calculate quality for Aged items - Will always increase
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int AgedItemQuality(Item item)
    {
        return ++item.Quality;
    }
    
    /// <summary>
    /// Calculate quality for Legendary items - Never alters
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int LegendaryItemQuality(Item item)
    {
        return item.Quality;
    }
    
    /// <summary>
    /// Calculate quality for Concert items - Value changes based on the SellIn value
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int ConcertItemQuality(Item item)
    {
        if (item.SellIn < 0)
            return 0;
        else if (item.SellIn <= 5)
            return item.Quality + 3;
        else if (item.SellIn <= 10)
            return item.Quality + 2;
        else
            return ++item.Quality;
    }
    
    /// <summary>
    /// Calculate quality for Conjured items - Degrades twice as fast as normal
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int ConjuredItemQuality(Item item)
    {
        return item.Quality - (2 * _qualityFactor);
    }
    
    /// <summary>
    /// Calculate quality for Normal items - Default degrade per day
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int NormalItemQuality(Item item)
    {
        return item.Quality - (1 * _qualityFactor);
    }
    #endregion
    
    /// <summary>
    /// Perform prerequisite operation for every items. This will be used to perform business logic later
    /// </summary>
    /// <param name="item"></param>
    private void Prerequisite(Item item)
    {
        string itemName = item.Name.Trim();
        
        // Assign ItemCategory
        _itemCategory = itemName switch
        {
            Constants.AgedBrie => ItemCategory.AgedItem,
            Constants.Sulfuras => ItemCategory.LegendaryItem,
            Constants.Backstage => ItemCategory.ConcertItem,
            Constants.Conjured => ItemCategory.ConjuredItem,
            _ => ItemCategory.NormalItem,
        };
    
        // Set Quality subtract factor
        _qualityFactor = item.SellIn > 0 ? 1 : 2;
    }

    /// <summary>
    /// Performs validation
    /// </summary>
    /// <param name="item"></param>
    private static void ValidateItem(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name.Trim()))
            Logger.LogWarning($"Item name {item.Name} is invalid");
    }

    public void UpdateQuality_Old()
    {
        for (var i = 0; i < _items.Count; i++)
        {
            if (_items[i].Name != "Aged Brie" && _items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
            {
                if (_items[i].Quality > 0)
                {
                    if (_items[i].Name != "Sulfuras, Hand of Ragnaros")
                    {
                        _items[i].Quality = _items[i].Quality - 1;
                    }
                }
            }
            else
            {
                if (_items[i].Quality < 50)
                {
                    _items[i].Quality = _items[i].Quality + 1;

                    if (_items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                    {
                        if (_items[i].SellIn < 11)
                        {
                            if (_items[i].Quality < 50)
                            {
                                _items[i].Quality = _items[i].Quality + 1;
                            }
                        }

                        if (_items[i].SellIn < 6)
                        {
                            if (_items[i].Quality < 50)
                            {
                                _items[i].Quality = _items[i].Quality + 1;
                            }
                        }
                    }
                }
            }

            if (_items[i].Name != "Sulfuras, Hand of Ragnaros")
            {
                _items[i].SellIn = _items[i].SellIn - 1;
            }

            if (_items[i].SellIn < 0)
            {
                if (_items[i].Name != "Aged Brie")
                {
                    if (_items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                    {
                        if (_items[i].Quality > 0)
                        {
                            if (_items[i].Name != "Sulfuras, Hand of Ragnaros")
                            {
                                _items[i].Quality = _items[i].Quality - 1;
                            }
                        }
                    }
                    else
                    {
                        _items[i].Quality = _items[i].Quality - _items[i].Quality;
                    }
                }
                else
                {
                    if (_items[i].Quality < 50)
                    {
                        _items[i].Quality = _items[i].Quality + 1;
                    }
                }
            }
        }
    }

}
