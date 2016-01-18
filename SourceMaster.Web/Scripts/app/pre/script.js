

function loadFrameContent(frame, targetPath) {
	frame.src = targetPath;
}

function loadFileBrowserContent(path) {
	var browser = $("#codeBrowserFrame");
	loadFrameContent(browser, path);
}

function onPageLoaded() {
	loadFileBrowserContent("SourceMaster/Syntax/FileSyntaxWalker.cs");
}
