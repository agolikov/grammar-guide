class BookmarkPayload {
  final String userId;
  final String guideId;
  final int unitIndex;
  final bool isAdded;

  BookmarkPayload(this.userId, this.guideId, this.unitIndex, this.isAdded);

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'guideId': guideId,
      'unitIndex': unitIndex,
      'isAdded': isAdded,
    };
  }
}