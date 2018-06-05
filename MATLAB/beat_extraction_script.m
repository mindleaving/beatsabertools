%path = 'C:\Users\Jan\Music\Red Hot Chili Peppers - Californication [Official Music Video].mp3';
%path = 'C:\Users\Jan\Music\Dr. Alban - Its My Life.mp3';
%path = 'C:\Users\Jan\Music\P!nk - Try.mp3';
%path = 'C:\Users\Jan\Music\Juli - Perfekte Welle.mp3';
path = 'C:\Users\Jan\Music\Taylor Swift - Safe  Sound.mp3';
%path = 'C:\Users\Jan\Music\Europe - The Final Countdown.mp3';
%path = 'C:\Users\Jan\Music\Survivor - Eye Of The Tiger.mp3';
%path = 'C:\Users\Jan\Music\Pur - Prinzessin  Funkelperlenaugen.mp3';
%path = 'C:\Users\Jan\Music\Arctic Monkeys - Mardy Bum.mp3';
% path = 'C:\Users\Jan\Music\Clarity - Zedd.mp3';
[ audioData, sampleRate ] = audioread(path);
downsampling = 8;
lowpassData = conv2(audioData,ones(downsampling,1)/downsampling,'same');
% load('lowpassFilter.mat');
% lowpassData = filter(lowpassFilter,audioData);
downsampledData = lowpassData(downsampling/2:downsampling:end,:);
player = audioplayer(downsampledData,sampleRate/downsampling);
player.play();

beatPositions = [];
while(player.isplaying)
    waitforbuttonpress;
    beatPositions = [ beatPositions; player.CurrentSample*downsampling];
end
plot(1:length(beatPositions),beatPositions);