using Xunit;
using System.Collections.Generic;
using GildedRoseKata;
using System.Linq;

namespace GildedRoseTests;

public class GildedRoseTest
{
    private readonly IList<Item> _items = new List<Item>();

    #region NormalItem
    [Fact]
    public void NormalItem_QualityZeroSellInMinus()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Elixir of the Mongoose", SellIn = 10, Quality = 5 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(0, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }

    [Fact]
    public void NormalItem_QualityDegradeTwiceSellInMinus()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Elixir of the Mongoose", SellIn = 10, Quality = 50 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(36, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }
    #endregion

    #region AgedBrie
    [Fact]
    public void AgedBrieItem_QualityIncrease()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Aged Brie", SellIn = 10, Quality = 10 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }

        // Old implementation returns 24.
        // Contradict in requirment between
        //                  ""Aged Brie" actually increases in Quality the older it gets"
        //                  and
        //                  "Once the sell by date has passed, Quality degrades twice as fast"
        Assert.Equal(22, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }

    [Fact]
    public void AgedBrieItem_QualityNeverMoreThanDefault()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Aged Brie", SellIn = 10, Quality = 40 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(50, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }

    [Fact]
    public void AgedBrieItem_QualityNeverMoreThanCustomQualityValue()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Aged Brie", SellIn = 10, Quality = 40 });
        GildedRose app = new GildedRose(_items, 45);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(45, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }
    #endregion

    #region Sulfuras
    [Fact]
    public void SulfurasItem_QualityAndSellInNeverChange()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 10, Quality = 70 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(70, _items[0].Quality);
        Assert.Equal(10, _items[0].SellIn);
    }
    #endregion

    #region Backstage
    [Fact]
    public void Backstage_QualityIncreasesWithFactor()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 2 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 8; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(21, _items[0].Quality);
        Assert.Equal(2, _items[0].SellIn);
    }

    [Fact]
    public void Backstage_QualitySetToZero()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 2 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 12; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(0, _items[0].Quality);
        Assert.Equal(-2, _items[0].SellIn);
    }
    #endregion

    #region Conjured
    [Fact]
    public void Conjured_QualityDegradeAsTwice()
    {
        _items.Clear();
        _items.Add(new Item { Name = "Conjured Mana Cake", SellIn = 10, Quality = 40 });
        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 10; i++)
        {
            app.UpdateQuality();
        }
        // This test case failed in old implementation as "Conjured" item has not been implemented
        Assert.Equal(20, _items[0].Quality);
        Assert.Equal(0, _items[0].SellIn);
    }
    #endregion

    #region Big transaction - Given items
    [Fact]
    public void BigTransaction()
    {
        _items.Clear();
        _items.Add(new Item { Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20 });
        _items.Add(new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 });
        _items.Add(new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7 });
        _items.Add(new Item { Name = "  ", SellIn = 0, Quality = 80 });
        _items.Add(new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80 });
        _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20 });
        _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 49 });
        _items.Add(new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49 });
        _items.Add(new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 });

        GildedRose app = new GildedRose(_items);
        for (var i = 0; i < 31; i++)
        {
            app.UpdateQuality();
        }
        Assert.Equal(0, _items[0].Quality);
        Assert.Equal(-21, _items[0].SellIn);

        Assert.Equal(31, _items[1].Quality);
        Assert.Equal(-29, _items[1].SellIn);

        Assert.Equal(0, _items[2].Quality);
        Assert.Equal(-26, _items[2].SellIn);

        Assert.Equal(0, _items[3].Quality);
        Assert.Equal(-31, _items[3].SellIn);

        Assert.Equal(80, _items[4].Quality);
        Assert.Equal(-1, _items[4].SellIn);

        Assert.Equal(0, _items[5].Quality);
        Assert.Equal(-16, _items[5].SellIn);

        Assert.Equal(0, _items[6].Quality);
        Assert.Equal(-21, _items[6].SellIn);

        Assert.Equal(0, _items[7].Quality);
        Assert.Equal(-26, _items[7].SellIn);

        Assert.Equal(0, _items[8].Quality);
        Assert.Equal(-28, _items[8].SellIn);
    }
    #endregion
}