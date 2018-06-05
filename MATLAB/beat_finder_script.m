close all;

% if(~exist('OcarinaSpectrogram','var'))
%     songSpectrogram = dlmread('C:\Temp\Dimitri Vegas - Ocarina.spect',';');
% end
if(~exist('songSpectrogram','var'))
    songSpectrogram = dlmread('C:\temp\Juli - Perfekte Welle.spect',';'); 
end
logSpectrogram = log(songSpectrogram');
downsampledSpectrogram = conv2(logSpectrogram,ones(11,1)/11);
downsampledSpectrogram = downsampledSpectrogram(5:10:end,:);
isIncrease = [zeros(size(logSpectrogram,1),1), logSpectrogram(:,2:end) > logSpectrogram(:,1:end-1)];
increaseSum = sum(isIncrease,1);
normalizedIncrease = increaseSum / max(increaseSum(:));
dynamicThreshold = compute_dynamic_threshold(normalizedIncrease, floor(1.5*44100/1024),2,4);
beatPositions = find(normalizedIncrease >= dynamicThreshold);

subplot(2,1,1);
plot([normalizedIncrease', dynamicThreshold']); xlim([1000, 2000]);
hold on;
for beatIdx=1:length(beatPositions)
   beatPosition = beatPositions(beatIdx);
   line([beatPosition, beatPosition],[0,1],'Color','r');
end
hold off;
subplot(2,1,2);
imagesc(logSpectrogram); xlim([1000, 2000]);

[ signal, sampleRate ] = audioread('C:\Users\Jan\Music\Juli - Perfekte Welle.mp3');
player = audioplayer(signal,sampleRate);
player.TimerFcn = @(x,y) xlim(x.CurrentSample/1024 + [0,1000]);
player.play()

