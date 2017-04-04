using FileSystem.Internal;
using Shouldly;
using Xunit;

namespace FileSystem.Tests.Internal
{
	public class ExtensisonsTests
	{
		[Theory]
		[InlineData("FileCreated", "File Created")]
		[InlineData("ThisHasASingleLetter", "This Has A Single Letter")]
		[InlineData("ThisIDProperty", "This ID Property")]
		[InlineData("ThisTLAToo", "This TLA Too")]
		public void When_sentencing_a_string(string input, string expected)
		{
			input.ToSentence().ShouldBe(expected);
		}
	}
}
