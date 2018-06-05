function threshold = compute_dynamic_threshold(signal, windowSize, minPeakCount, maxPeakCount)
    if(mod(windowSize,2) == 1)
        windowSize = windowSize + 1;
    end
    halfWindowSize = windowSize/2;
    idxRange = halfWindowSize:halfWindowSize:length(signal)-halfWindowSize;
    downsampledThreshold = zeros(length(idxRange),2);
    for idx=1:length(idxRange)
        signalIdx = idxRange(idx);
        dataRange = signalIdx+(-halfWindowSize+1:halfWindowSize);
        dataInWindow = signal(dataRange);
        dataInWindow = sort(dataInWindow);
        minPeakThreshold = dataInWindow(end-(minPeakCount-1));
        maxPeakThreshold = dataInWindow(end-(maxPeakCount-1));
        combinedThreshold = 0.8*minPeakThreshold + 0.2*maxPeakThreshold;
        downsampledThreshold(idx,:) = [ signalIdx, combinedThreshold ];
    end
    threshold = interp1(downsampledThreshold(:,1),downsampledThreshold(:,2),1:length(signal));
end