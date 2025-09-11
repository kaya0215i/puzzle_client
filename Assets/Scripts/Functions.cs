using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class Functions {
    /// <summary>
    /// �ړI�̒l�ɍł��߂��l���擾(float�p)
    /// </summary>
    public static float Nearest(this IEnumerable<float> source, float targetValue) {
        if (source.Count() == 0) {
            Debug.LogError($"�l�������Ă��Ȃ��̂ŁA�ł��߂��l���擾�o���܂���");
            return targetValue;
        }

        //�ړI�̒l�Ƃ̍��̐�Βl���ŏ��̒l���v�Z
        var min = source.Min(value => Mathf.Abs(value - targetValue));

        //��Βl���ŏ��̒l�����������ł��߂��l�Ƃ��ĕԂ�
        return source.First(value => Mathf.Approximately(Mathf.Abs(value - targetValue), min));
    }
}
