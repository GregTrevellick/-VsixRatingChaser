﻿using System;
using VsixRatingChaser.Dtos;
using VsixRatingChaser.Enums;
using VsixRatingChaser.Interfaces;

namespace VsixRatingChaser
{
    public class Chaser : IChaser
    {
        private bool _ratingHyperLinkClicked;

        public ChaseOutcomeDto Chase(IRatingDetailsDto ratingDetailsDto, ExtensionDetailsDto extensionDetailsDto)
        {
            var outcome = Validate(extensionDetailsDto);

            if (!outcome.Rejected)
            {
                var ratingDecider = new RatingDecider();
                var shouldShowDialog = ratingDecider.ShouldShowDialog(ratingDetailsDto);

                if (shouldShowDialog)
                {
                    ShowDialog(ratingDetailsDto, extensionDetailsDto);
                    outcome.ReviewRequestDialogShown = true;
                    outcome.MarketplaceHyperLinkClicked = _ratingHyperLinkClicked;
                }
            }

            return outcome;
        }

        private ChaseOutcomeDto Validate(ExtensionDetailsDto extensionDetailsDto)
        {
            var outcome = new ChaseOutcomeDto();

            if (string.IsNullOrWhiteSpace(extensionDetailsDto.AuthorName))
            {
                outcome.Rejected = true;
                outcome.RejectionReason = RejectionReason.AuthorNameCannotBeBlank;
            }

            if (string.IsNullOrWhiteSpace(extensionDetailsDto.ExtensionName))
            {
                outcome.Rejected = true;
                outcome.RejectionReason = RejectionReason.ExtensionNameCannotBeBlank;
            }

            if (string.IsNullOrWhiteSpace(extensionDetailsDto.MarketPlaceUrl))
            {
                outcome.Rejected = true;
                outcome.RejectionReason = RejectionReason.MarketplaceUrlUndefined;
            }

            if (!extensionDetailsDto.MarketPlaceUrl.ToLower()
                .StartsWith("https://marketplace.visualstudio.com/items?itemName=".ToLower()))
            {
                outcome.Rejected = true;
                outcome.RejectionReason = RejectionReason.MarketplaceUrlStartIsWrong;
            }

            return outcome;
        }

        private void ShowDialog(IRatingDetailsDto ratingDetailsDto, ExtensionDetailsDto extensionDetailsDto)
        {
            var ratingDialog = new RatingDialog(extensionDetailsDto);
            ratingDialog.Show();
            _ratingHyperLinkClicked = ratingDialog.RatingHyperLinkClicked;
            PersistRatingDetails(ratingDetailsDto);
        }

        private void PersistRatingDetails(IRatingDetailsDto ratingDetailsDto)
        {
            ratingDetailsDto.LastRatingRequest = DateTime.Now;
            ratingDetailsDto.RatingRequestCount++;
            ratingDetailsDto.SaveSettingsToStorage();
        }
    }
}