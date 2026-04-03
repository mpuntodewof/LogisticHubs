using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

[Collection("E2E")]
public class Flow06_MarketingCampaignTests : NiagaOneTestBase
{
    [Fact]
    public async Task Marketing_Should_Create_Promotion_Coupon_And_Loyalty_Program()
    {
        // ── Login as marketing user ──────────────────────────────────────
        var token = await LoginAsync("marketing@niagaone.com", "Password123!");

        // ── Create promotion ─────────────────────────────────────────────
        var now = DateTime.UtcNow;
        var promoResponse = await AuthPost(token, "/api/promotions", new
        {
            name = "Flash Sale Electronics",
            code = "FLASH-E2E",
            promotionType = "FlashSale",
            discountType = "Percentage",
            discountValue = 20,
            startDate = now,
            endDate = now.AddDays(7)
        });
        promoResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var promo = await ReadAs<JsonElement>(promoResponse);
        var promoId = promo.GetProperty("id").GetString()!;
        promoId.Should().NotBeNullOrEmpty();

        // ── Add rule to promotion ────────────────────────────────────────
        var electronicsId = TestDataStore.GetGuid("electronicsId").ToString();
        var ruleResponse = await AuthPost(token, $"/api/promotions/{promoId}/rules", new
        {
            ruleType = "SpecificCategories",
            categoryId = electronicsId
        });
        ruleResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // ── Activate promotion ───────────────────────────────────────────
        var activateResponse = await AuthPost(token, $"/api/promotions/{promoId}/activate");
        activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // ── Create coupon ────────────────────────────────────────────────
        var couponResponse = await AuthPost(token, "/api/coupons", new
        {
            code = "WELCOME-E2E",
            discountType = "Percentage",
            discountValue = 10,
            minimumOrderAmount = 100000,
            startDate = now,
            endDate = now.AddDays(365),
            maxUsageCount = 100
        });
        couponResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var coupon = await ReadAs<JsonElement>(couponResponse);
        var couponId = coupon.GetProperty("id").GetString()!;
        couponId.Should().NotBeNullOrEmpty();

        // ── Create loyalty program ───────────────────────────────────────
        var loyaltyResponse = await AuthPost(token, "/api/loyalty-programs", new
        {
            name = "NiagaOne Rewards",
            pointsPerIdrSpent = 0.001,
            redemptionRateIdr = 100,
            minRedemptionPoints = 100
        });
        loyaltyResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loyalty = await ReadAs<JsonElement>(loyaltyResponse);
        var loyaltyProgramId = loyalty.GetProperty("id").GetString()!;
        loyaltyProgramId.Should().NotBeNullOrEmpty();

        // ── Create 3 tiers ───────────────────────────────────────────────
        var tiers = new[]
        {
            new { name = "Bronze", minimumPoints = 0, multiplier = 1.0 },
            new { name = "Silver", minimumPoints = 1000, multiplier = 1.5 },
            new { name = "Gold", minimumPoints = 5000, multiplier = 2.0 }
        };

        foreach (var tier in tiers)
        {
            var tierResponse = await AuthPost(token, $"/api/loyalty-programs/{loyaltyProgramId}/tiers", tier);
            tierResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // ── Enroll customer in loyalty program ───────────────────────────
        var customerId = TestDataStore.GetGuid("customerId").ToString();
        var enrollResponse = await AuthPost(token, $"/api/loyalty-programs/{loyaltyProgramId}/members", new
        {
            customerId
        });
        enrollResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var membership = await ReadAs<JsonElement>(enrollResponse);
        var membershipId = membership.GetProperty("id").GetString()!;
        membershipId.Should().NotBeNullOrEmpty();

        // ── Store IDs for subsequent flows ───────────────────────────────
        TestDataStore.Set("promoId", promoId);
        TestDataStore.Set("couponId", couponId);
        TestDataStore.Set("loyaltyProgramId", loyaltyProgramId);
        TestDataStore.Set("membershipId", membershipId);
    }
}
